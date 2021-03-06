using Microsoft.Extensions.DependencyInjection;
using Middlewares.Tests.TestMiddlewares;
using NUnit.Framework;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Middlewares.Tests
{
    /// <summary>
    /// Testing pipeline builder functionality.
    /// </summary>
    [TestOf(typeof(PipelineBuilder<>))]
    public class PipelineBuilderTests
    {
        /// <summary>
        /// Null checks.
        /// </summary>
        [Test]
        public void Should_Throw_ArgumentNullException_When_Invalid_Middleware_Provided()
        {
            // arrange
            var services = new ServiceCollection();

            var pipelineBuilder = services.ConfigurePipelineFor<TestCtx>();

            // act
            void TestCode() => pipelineBuilder.Use(middleware: null!);

            // assert
            Assert.Throws<ArgumentNullException>(TestCode);
        }

        /// <summary>
        /// Null checks.
        /// </summary>
        [Test]
        public void Should_Throw_ArgumentNullException_When_Invalid_MiddlewareDelegate_Provided()
        {
            // arrange
            var services = new ServiceCollection();

            var pipelineBuilder = services.ConfigurePipelineFor<TestCtx>();

            FuncAsNextMiddlewareDelegateWithServiceProvider<TestCtx> middleware = null;

            // act
            void TestCode() => pipelineBuilder.Use(middleware!);

            // assert
            Assert.Throws<ArgumentNullException>(TestCode);
        }

        /// <summary>
        /// Null checks.
        /// </summary>
        [Test]
        public void Should_Throw_ArgumentNullException_When_Invalid_MiddlewareDelegate_Provided_Without_ServiceProvider()
        {
            // arrange
            var services = new ServiceCollection();

            var pipelineBuilder = services.ConfigurePipelineFor<TestCtx>();

            FuncAsNextMiddlewareDelegate<TestCtx> middleware = null;

            // act
            void TestCode() => pipelineBuilder.Use(middleware!);

            // assert
            Assert.Throws<ArgumentNullException>(TestCode);
        }

        /// <summary>
        /// Null checks.
        /// </summary>
        [Test]
        public void Should_Throw_ArgumentNullException_When_Invalid_MiddlewareFactory_Provided_Without_ServiceProvider()
        {
            // arrange
            var services = new ServiceCollection();

            var pipelineBuilder = services.ConfigurePipelineFor<TestCtx>();

            ParameterAsNextMiddlewareFactoryDelegate<TestCtx> middleware = null;

            // act
            void TestCode() => pipelineBuilder.Use(middleware!);

            // assert
            Assert.Throws<ArgumentNullException>(TestCode);
        }

        /// <summary>
        /// Null checks.
        /// </summary>
        [Test]
        public void Should_Throw_ArgumentNullException_When_Invalid_MiddlewareFactory_Provided()
        {
            // arrange
            var services = new ServiceCollection();

            var pipelineBuilder = services.ConfigurePipelineFor<TestCtx>();

            ParameterWithServiceProviderAsNextMiddlewareFactoryDelegate<TestCtx> middleware = null;

            // act
            void TestCode() => pipelineBuilder.Use(middleware!);

            // assert
            Assert.Throws<ArgumentNullException>(TestCode);
        }

        /// <summary>
        /// <see cref="PipelineBuilder{TParameter}.Build(IServiceProvider)"/> creates <see cref="IPipeline{TParameter}"/>
        /// with current set of <see cref="PipelineComponent{TParameter}"/>, and should not affect on further PipelineBuilder changes.
        /// </summary>
        [Test]
        public async Task Pipeline_Should_Preserve_OwnPipelineComponents_AfterBuild()
        {
            // arrange
            var sp = new ServiceCollection().BuildServiceProvider();

            var pipelineBuilder = new PipelineBuilder<TestCtx>();

            pipelineBuilder.Use<Middleware1>();

            var firstPipeline = pipelineBuilder.Build(sp);

            pipelineBuilder.Use(async (ctx, next, _) =>
            {
                ctx.Msg += "Before_LambdaMiddleware";

                await next();

                ctx.Msg += "After_LambdaMiddleware";
            });
            var secondPipeline = pipelineBuilder.Build(sp);

            var testContext = new TestCtx();

            // act
            await firstPipeline.ExecuteAsync(testContext); // this pipeline has no lambda middleware in pipeline.

            Assert.AreEqual("Before_Middleware1After_Middleware1", testContext.Msg);

            testContext.Msg = null; // clear before second pipeline execution

            await secondPipeline.ExecuteAsync(testContext); // this has full pipeline.

            // assert
            Assert.AreEqual(
                "Before_Middleware1Before_LambdaMiddlewareAfter_LambdaMiddlewareAfter_Middleware1",
                testContext.Msg
            );
        }

        /// <summary>
        /// We should allow to use pipeline builder without service provider, if it not used in any of middlewares.
        /// </summary>
        [Test]
        public async Task Pipeline_Executes_Without_ServiceProvider()
        {
            // arrange
            var pipelineBuilder = new PipelineBuilder<TestCtx>();

            pipelineBuilder
                .Use(new Middleware1())
                .Use(async (ctx, next, _) =>
                {
                    ctx.Msg += "Before_LambdaMiddleware";

                    await next();

                    ctx.Msg += "After_LambdaMiddleware";
                })
                .UseWhen(_ => true, async (ctx, next, _) =>
                {
                    ctx.Msg += "Before_ConditionalLambdaMiddleware";

                    await next();

                    ctx.Msg += "After_ConditionalLambdaMiddleware";
                });

            var pipeline = pipelineBuilder.Build();

            var testContext = new TestCtx();

            // act
            await pipeline.ExecuteAsync(testContext);

            // assert
            Assert.AreEqual(
                "Before_Middleware1Before_LambdaMiddlewareBefore_ConditionalLambdaMiddlewareAfter_ConditionalLambdaMiddlewareAfter_LambdaMiddlewareAfter_Middleware1",
                testContext.Msg
            );
        }

        /// <summary>
        /// We should not allow to use pipeline builder without service provider, if it required in one of middlewares.
        /// </summary>
        [Test]
        public void Pipeline_Should_Throw_ArgumentNullException_When_It_Cannot_Be_Executed_Without_ServiceProvider()
        {
            // arrange
            var pipelineBuilder = new PipelineBuilder<TestCtx>();

            pipelineBuilder.Use<Middleware1>();

            // act & assert
            Assert.Throws<ArgumentNullException>(() => pipelineBuilder.Build());
        }

        /// <summary>
        /// Test, that checks using of middleware delegate <see cref="IPipelineBuilder{TParameter}.Use(FuncAsNextMiddlewareDelegate{TParameter})"/>
        /// </summary>
        [Test]
        public async Task Executes_With_Register_MiddlewareDelegate()
        {
            // arrange
            var services = new ServiceCollection();

            services
                .ConfigurePipelineFor<TestCtx>()
                .Use(next =>
                {
                    return (ctx, cancellationToken) =>
                    {
                        ctx.Msg = "123";

                        return next(ctx, cancellationToken);
                    };
                });

            var sp = services.BuildServiceProvider();

            var pipeline = sp.GetRequiredService<IPipeline<TestCtx>>();

            var testCtx = new TestCtx();

            // act
            await pipeline.ExecuteAsync(testCtx);

            // assert
            Assert.AreEqual("123", testCtx.Msg);
        }

        /// <summary>
        /// Test, that checks using of middleware delegate without service provider.
        /// </summary>
        [Test]
        public void Can_Register_MiddlewareDelegate_Without_ServiceProvider()
        {
            // arrange
            var pipelineBuilder = new PipelineBuilder<TestCtx>();

            pipelineBuilder.Use((_, next, _) => next());

            // act
            Task TestCode()
            {
                var pipeline = pipelineBuilder.Build();
                return pipeline.ExecuteAsync(new TestCtx(), CancellationToken.None);
            }

            // assert
            Assert.DoesNotThrowAsync(TestCode);
        }

        /// <summary>
        /// Test, that checks using of middleware delegate without serviceProvider
        /// <see cref="IPipelineBuilder{TParameter}.Use(ParameterAsNextMiddlewareFactoryDelegate{TParameter})"/>.
        /// </summary>
        [Test]
        public async Task Pipeline_Executes_MiddlewareFactoryDelegate_Without_ServiceProvider()
        {
            // arrange
            var pipelineBuilder = new PipelineBuilder<TestCtx>();

            pipelineBuilder
                .Use(_ => new Middleware1())
                .Use(async (ctx, next, _) =>
                {
                    ctx.Msg += "Before_LambdaMiddleware";

                    await next();

                    ctx.Msg += "After_LambdaMiddleware";
                })
                .UseWhen(_ => true, async (ctx, next, _) =>
                {
                    ctx.Msg += "Before_ConditionalLambdaMiddleware";

                    await next();

                    ctx.Msg += "After_ConditionalLambdaMiddleware";
                });

            var pipeline = pipelineBuilder.Build();

            var testContext = new TestCtx();

            // act
            await pipeline.ExecuteAsync(testContext);

            // assert
            Assert.AreEqual(
                "Before_Middleware1Before_LambdaMiddlewareBefore_ConditionalLambdaMiddlewareAfter_ConditionalLambdaMiddlewareAfter_LambdaMiddlewareAfter_Middleware1",
                testContext.Msg
            );
        }

        /// <summary>
        /// Test, that checks using of middleware delegate with serviceProvider
        /// <see cref="IPipelineBuilder{TParameter}.Use(ParameterWithServiceProviderAsNextMiddlewareFactoryDelegate{TParameter})"/>.
        /// </summary>
        [Test]
        public async Task Pipeline_Executes_MiddlewareFactoryDelegate_With_ServiceProvider()
        {
            // arrange
            var pipelineBuilder = new PipelineBuilder<TestCtx>();

            pipelineBuilder
                .Use((_, _) => new Middleware1())
                .Use(async (ctx, next, _) =>
                {
                    ctx.Msg += "Before_LambdaMiddleware";

                    await next();

                    ctx.Msg += "After_LambdaMiddleware";
                })
                .UseWhen(_ => true, async (ctx, next, _) =>
                {
                    ctx.Msg += "Before_ConditionalLambdaMiddleware";

                    await next();

                    ctx.Msg += "After_ConditionalLambdaMiddleware";
                });

            var pipeline = pipelineBuilder.Build(new ServiceCollection().BuildServiceProvider());

            var testContext = new TestCtx();

            // act
            await pipeline.ExecuteAsync(testContext);

            // assert
            Assert.AreEqual(
                "Before_Middleware1Before_LambdaMiddlewareBefore_ConditionalLambdaMiddlewareAfter_ConditionalLambdaMiddlewareAfter_LambdaMiddlewareAfter_Middleware1",
                testContext.Msg
            );
        }
    }
}