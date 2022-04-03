using Microsoft.Extensions.DependencyInjection;
using Middlewares.Tests.TestMiddlewares;
using NUnit.Framework;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Middlewares.Tests
{
    /// <summary>
    /// Pipeline builder additional extension methods tests.
    /// </summary>
    [TestOf(typeof(PipelineBuilderConditionalExtensions))]
    public class PipelineBuilderConditionalExtensionsTests
    {
        /// <summary>
        /// Ensures, that we executing middlewares in correct order.
        /// </summary>
        [Test]
        public async Task Should_BeCorrect_PipelineExecutionOrder_WithSimpleConditions([Values(true, false)] bool executeMiddleware2)
        {
            // arrange
            var pipelineBuilder = new PipelineBuilder<TestCtx>();

            pipelineBuilder.Use<Middleware1>();
            pipelineBuilder.UseWhen<TestCtx, Middleware2>(ctx => ctx.ExecuteMiddleware2);
            pipelineBuilder.Use(async (ctx, next, _) =>
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

        /// <summary>
        /// Ensures, that we executing middlewares in correct order.
        /// </summary>
        [Test]
        public async Task Should_BeCorrect_PipelineExecutionOrder_WithServiceProviderConditions([Values(true, false)] bool executeMiddleware2)
        {
            // arrange
            var pipelineBuilder = new PipelineBuilder<TestCtx>();

            pipelineBuilder.Use<Middleware1>();
            pipelineBuilder.UseWhen(ctx => ctx.ExecuteMiddleware2, async (_, ctx, next, _) =>
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

        /// <summary>
        /// Null checks.
        /// </summary>
        [Test]
        public void Cannot_Register_Invalid_Conditional_MiddlewareDelegate_WithDI(
            [Values(true, false)] bool predicateResult,
            [Values(true, false)] bool passNullToPredicate
        )
        {
            // arrange
            var services = new ServiceCollection();
            services.AddScoped<Middleware2>();

            var pipelineBuilder = services.ConfigurePipelineFor<TestCtx>();

            Func<TestCtx, bool> predicate = passNullToPredicate
                ? null
                : _ => predicateResult;
            // act
            void TestCode() => pipelineBuilder.UseWhen<TestCtx, Middleware2>(predicate!);

            // assert
            if (passNullToPredicate)
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

        /// <summary>
        /// Null checks.
        /// </summary>
        [Test]
        public void Cannot_Register_Invalid_Conditional_MiddlewareDelegate(
            [Values(true, false)]bool predicateResult,
            [Values(true, false)]bool passNull,
            [Values(true, false)]bool passNullToPredicate
        )
        {
            // arrange
            var services = new ServiceCollection();

            var pipelineBuilder = services.ConfigurePipelineFor<TestCtx>();

            Func<IServiceProvider, TestCtx, Func<Task>, CancellationToken, Task> middleware = passNull
                ? null
                : (_, _, _, _) => Task.CompletedTask;

            Func<TestCtx, bool> predicate = passNullToPredicate
                ? null
                : _ => predicateResult;

            // act
            void TestCode() => pipelineBuilder.UseWhen(predicate!, middleware!);

            // assert
            if (passNull || passNullToPredicate)
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

        /// <summary>
        /// Null checks.
        /// </summary>
        [Test]
        public void Cannot_Register_Invalid_Conditional_MiddlewareDelegate_Without_ServiceProvider(
            [Values(true, false)] bool predicateResult,
            [Values(true, false)] bool passNull,
            [Values(true, false)] bool passNullToPredicate
        )
        {
            // arrange
            var services = new ServiceCollection();

            var pipelineBuilder = services.ConfigurePipelineFor<TestCtx>();

            Func<TestCtx, NextMiddleware, CancellationToken, Task> middleware = passNull
                ? null
                : (_, _, _) => Task.CompletedTask;

            Func<TestCtx, bool> predicate = passNullToPredicate
                ? null
                : _ => predicateResult;

            // act
            void TestCode() => pipelineBuilder.UseWhen(predicate!, middleware!);

            // assert
            if (passNull || passNullToPredicate)
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