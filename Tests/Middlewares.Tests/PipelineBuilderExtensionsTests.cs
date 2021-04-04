using Microsoft.Extensions.DependencyInjection;
using Middlewares.Tests.TestMiddlewares;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace Middlewares.Tests
{
    [TestOf(typeof(PipelineBuilderExtensions))]
    public class PipelineBuilderExtensionsTests
    {
        [Test]
        [TestCase(null, typeof(ArgumentNullException))]
        [TestCase(typeof(TestCtx), typeof(ArgumentException))]
        public void Cannot_Register_InvalidType(Type typeToRegister, Type exceptionType)
        {
            // arrange
            var services = new ServiceCollection();

            var pipelineBuilder = services.ConfigurePipelineFor<TestCtx>();

            // act
            void TestCode() => pipelineBuilder.Use(typeToRegister);

            // assert
            Assert.Throws(exceptionType, TestCode);
        }

        [Test]
        public void Cannot_Register_Invalid_Func()
        {
            // arrange
            var services = new ServiceCollection();

            var pipelineBuilder = services.ConfigurePipelineFor<TestCtx>();

            Func<TestCtx, Func<Task>, Task> middleware = null;

            // act
            void TestCode() => pipelineBuilder.Use(middleware!);

            // assert
            Assert.Throws<ArgumentNullException>(TestCode);
        }

        [Test]
        public void Cannot_Register_Invalid_MiddlewareDelegate()
        {
            // arrange
            var services = new ServiceCollection();

            var pipelineBuilder = services.ConfigurePipelineFor<TestCtx>();

            Func<MiddlewareDelegate<TestCtx>, MiddlewareDelegate<TestCtx>> middleware = null;

            // act
            void TestCode() => pipelineBuilder.Use(middleware!);

            // assert
            Assert.Throws<ArgumentNullException>(TestCode);
        }

        [Test]
        public async Task NoOp_WhenPipeline_Empty()
        {
            // arrange
            var services = new ServiceCollection();

            services.ConfigurePipelineFor<TestCtx>();

            var sp = services.BuildServiceProvider();

            var pipeline = sp.GetRequiredService<IPipeline<TestCtx>>();

            var testCtx = new TestCtx();

            // act
            await pipeline.ExecuteAsync(testCtx);

            // assert
            Assert.IsNull(testCtx.Msg);
        }

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
    }
}