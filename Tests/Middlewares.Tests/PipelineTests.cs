using Microsoft.Extensions.DependencyInjection;
using Middlewares.Tests.TestMiddlewares;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Middlewares.Tests
{
    /// <summary>
    /// Pipeline execution order tests.
    /// </summary>
    [TestOf(typeof(Pipeline<>))]
    public class PipelineTests
    {
        [Test]
        public async Task Should_BeCorrect_PipelineExecutionOrder()
        {
            // arrange
            var pipelineBuilder = new PipelineBuilder<TestCtx>();

            pipelineBuilder.Use<Middleware1>();
            pipelineBuilder.Use<Middleware2>();
            pipelineBuilder.Use(async (ctx, next) =>
            {
                ctx.ExecutedMiddlewaresCount++;

                ctx.Msg += "Before_LambdaMiddleware";

                await next();
                ctx.Msg += "After_LambdaMiddleware";
            });

            var pipeline = new Pipeline<TestCtx>(new ServiceCollection().BuildServiceProvider(), pipelineBuilder);

            var testContext = new TestCtx();
            Assert.IsNull(testContext.Msg);
            Assert.Zero(testContext.ExecutedMiddlewaresCount);

            // act
            await pipeline.ExecuteAsync(testContext);

            // assert
            const string expectedMessage = "Before_" + nameof(Middleware1) +
                                           "Before_" + nameof(Middleware2) +
                                           "Before_LambdaMiddleware" +
                                           "After_LambdaMiddleware" +
                                           "After_" + nameof(Middleware2) +
                                           "After_" + nameof(Middleware1);

            Assert.AreEqual(expectedMessage, testContext.Msg, "Pipeline execution order is not match");
            Assert.AreEqual(3, testContext.ExecutedMiddlewaresCount, "ExecutedMiddlewaresCount is not match");
        }

        [Test]
        public async Task Should_BeCorrect_PipelineExecutionOrder_WithDeps()
        {
            // arrange
            var services = new ServiceCollection();
            var dep = new ExampleDependency();
            services.AddSingleton(dep);

            var sp = services.BuildServiceProvider();

            var pipelineBuilder = new PipelineBuilder<TestCtx>();

            pipelineBuilder.Use<Middleware1>();
            pipelineBuilder.Use<Middleware2>();
            pipelineBuilder.Use<TestCtx, ExampleDependency>(async (ctx, deps, next) =>
            {
                ctx.ExecutedMiddlewaresCount++;
                deps.Resolved = true;

                ctx.Msg += "Before_LambdaMiddleware";

                await next();
                ctx.Msg += "After_LambdaMiddleware";
            });

            var pipeline = new Pipeline<TestCtx>(sp, pipelineBuilder);

            var testContext = new TestCtx();

            Assert.IsFalse(dep.Resolved);

            // act
            await pipeline.ExecuteAsync(testContext);

            // assert
            Assert.IsTrue(dep.Resolved, "Dependency is not resolved");
        }

        [Test]
        public void Should_Throw_If_Invalid_Component()
        {
            // arrange
            var pipelineInfoAccessorMock = new Mock<IPipelineInfoAccessor<TestCtx>>();
            pipelineInfoAccessorMock.Setup(d => d.PipelineComponents)
                .Returns(new List<PipelineComponents<TestCtx>>
                {
                    new() // invalid instance
                });

            var pipeline = new Pipeline<TestCtx>(null!, pipelineInfoAccessorMock.Object);

            // act
            Task TestCode() => pipeline.ExecuteAsync(new TestCtx());

            // assert
            Assert.ThrowsAsync<InvalidOperationException>(TestCode);
        }
    }
}