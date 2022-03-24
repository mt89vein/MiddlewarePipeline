using Microsoft.Extensions.DependencyInjection;
using Middlewares.Tests.TestMiddlewares;
using NUnit.Framework;
using System;
using System.Threading;
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

            Func<TestCtx, NextMiddleware, CancellationToken, Task> middleware = null;

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
        public void Executes_With_MiddlewareDelegate()
        {
            // arrange
            var pipelineBuilder = new PipelineBuilder<TestCtx>();

            pipelineBuilder.Use(middlewareDelegate: next => next);

            var pipeline = pipelineBuilder.Build();

            // act
            Task TestCode() => pipeline.ExecuteAsync(new TestCtx(), CancellationToken.None);

            // assert
            Assert.DoesNotThrowAsync(TestCode);
        }
    }
}