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
    [TestOf(typeof(PipelineBuilderExtensions))]
    public class PipelineBuilderExtensionsTests
    {
        /// <summary>
        /// Ensures, that we can register correct types, that implements <see cref="IMiddleware{TParameter}"/> interface.
        /// </summary>
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

        /// <summary>
        /// Null checks.
        /// </summary>
        [Test]
        public void Should_Throw_ArgumentNullException_If_Null_Passed_As_Middleware_Func()
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

        /// <summary>
        /// Empty pipeline is also valid pipeline.
        /// </summary>
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

        /// <summary>
        /// We can also use delegate as middleware.
        /// </summary>
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