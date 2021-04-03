using Microsoft.Extensions.DependencyInjection;
using Middlewares.Tests.TestMiddlewares;
using NUnit.Framework;
using System;

namespace Middlewares.Tests
{
    [TestOf(typeof(PipelineBuilder<>))]
    public class PipelineBuilderTests
    {
        [Test]
        public void Cannot_Register_Invalid_MiddlewareDelegate()
        {
            // arrange
            var services = new ServiceCollection();

            var pipelineBuilder = services.ConfigurePipelineFor<TestCtx>();

            Func<IServiceProvider, MiddlewareDelegate<TestCtx>, MiddlewareDelegate<TestCtx>> middleware = null;

            // act
            void TestCode() => pipelineBuilder.Use(middleware!);

            // assert
            Assert.Throws<ArgumentNullException>(TestCode);
        }
    }
}