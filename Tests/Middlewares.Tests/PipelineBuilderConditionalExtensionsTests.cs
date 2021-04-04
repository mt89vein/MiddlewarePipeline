using Microsoft.Extensions.DependencyInjection;
using Middlewares.Tests.TestMiddlewares;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace Middlewares.Tests
{
    [TestOf(typeof(PipelineBuilderConditionalExtensions))]
    public class PipelineBuilderConditionalExtensionsTests
    {
        [Test]
        public async Task Should_BeCorrect_PipelineExecutionOrder_WithSimpleConditions([Values(true, false)] bool executeMiddleware2)
        {
            // arrange
            var pipelineBuilder = new PipelineBuilder<TestCtx>();

            pipelineBuilder.Use<Middleware1>();
            pipelineBuilder.UseWhen<TestCtx, Middleware2>(ctx => ctx.ExecuteMiddleware2);
            pipelineBuilder.Use(async (ctx, next) =>
            {
                ctx.ExecutedMiddlewaresCount++;

                ctx.Msg += "Before_LambdaMiddleware";

                await next();
                ctx.Msg += "After_LambdaMiddleware";
            });

            var pipeline = pipelineBuilder.Build(new ServiceCollection().BuildServiceProvider());

            var testContext = new TestCtx
            {
                ExecuteMiddleware2 = executeMiddleware2
            };

            Assert.IsNull(testContext.Msg);
            Assert.Zero(testContext.ExecutedMiddlewaresCount);

            // act
            await pipeline.ExecuteAsync(testContext);

            // assert
            var expectedMessage = "Before_" + nameof(Middleware1) +
                                  (executeMiddleware2 ? "Before_" + nameof(Middleware2) : string.Empty) +
                                  "Before_LambdaMiddleware" +
                                  "After_LambdaMiddleware" +
                                  (executeMiddleware2 ? "After_" + nameof(Middleware2) : string.Empty) +
                                  "After_" + nameof(Middleware1);

            var expectedExecutedMiddlewaresCount = executeMiddleware2 ? 3 : 2;

            Assert.AreEqual(expectedMessage, testContext.Msg, "Pipeline execution order is not match");
            Assert.AreEqual(expectedExecutedMiddlewaresCount, testContext.ExecutedMiddlewaresCount, "ExecutedMiddlewaresCount is not match");
        }

        [Test]
        public async Task Should_BeCorrect_PipelineExecutionOrder_WithServiceProviderConditions([Values(true, false)] bool executeMiddleware2)
        {
            // arrange
            var pipelineBuilder = new PipelineBuilder<TestCtx>();

            pipelineBuilder.Use<Middleware1>();
            pipelineBuilder.UseWhen(ctx => ctx.ExecuteMiddleware2, async (_, ctx, next) =>
            {
                ctx.ExecutedMiddlewaresCount++;

                ctx.Msg += "Before_LambdaMiddleware";

                await next();

                ctx.Msg += "After_LambdaMiddleware";
            });
            pipelineBuilder.Use<Middleware2>();

            var pipeline = pipelineBuilder.Build(new ServiceCollection().BuildServiceProvider());

            var testContext = new TestCtx
            {
                ExecuteMiddleware2 = executeMiddleware2
            };

            Assert.IsNull(testContext.Msg);
            Assert.Zero(testContext.ExecutedMiddlewaresCount);

            // act
            await pipeline.ExecuteAsync(testContext);

            // assert
            var expectedMessage = "Before_" + nameof(Middleware1) +
                                  (executeMiddleware2 ? "Before_LambdaMiddleware" : string.Empty) +
                                  "Before_" + nameof(Middleware2) +

                                  "After_" + nameof(Middleware2) +
                                  (executeMiddleware2 ? "After_LambdaMiddleware" : string.Empty) +
                                  "After_" + nameof(Middleware1);

            var expectedExecutedMiddlewaresCount = executeMiddleware2 ? 3 : 2;

            Assert.AreEqual(expectedMessage, testContext.Msg, "Pipeline execution order is not match");
            Assert.AreEqual(expectedExecutedMiddlewaresCount, testContext.ExecutedMiddlewaresCount, "ExecutedMiddlewaresCount is not match");
        }

        [Test]
        public void Cannot_Register_Invalid_Conditional_MiddlewareDelegate(
            [Values(true, false)]bool predicateResult,
            [Values(true, false)]bool passNull
        )
        {
            // arrange
            var services = new ServiceCollection();

            var pipelineBuilder = services.ConfigurePipelineFor<TestCtx>();

            Func<IServiceProvider, TestCtx, Func<Task>, Task> middleware = passNull
                ? null
                : (_, _, _) => Task.CompletedTask;

            // act
            void TestCode() => pipelineBuilder.UseWhen(_ => predicateResult, middleware!);

            // assert
            if (passNull)
            {
                Assert.Throws<ArgumentNullException>(TestCode);
            }
            else
            {
                Assert.DoesNotThrow(TestCode);

                Assert.DoesNotThrowAsync(async () =>
                {
                    var sp = services.BuildServiceProvider();
                    await sp.GetRequiredService<IPipeline<TestCtx>>().ExecuteAsync(new TestCtx());
                });
            }
        }
    }
}