using Microsoft.Extensions.DependencyInjection;
using Middlewares;
using NUnit.Framework;
using System;
using UnitTests.TestMiddlewares;

namespace UnitTests
{
    [TestOf(typeof(PipelineBuilder<>))]
    public class PipelineBuilderTests
    {
        [Test]
        public void Cannot_Register_Invalid_MiddlewareDelegate()
        {
            var services = new ServiceCollection();

            var pipelineBuilder = services.ConfigurePipelineFor<TestCtx>();

            Func<IServiceProvider, MiddlewareDelegate<TestCtx>, MiddlewareDelegate<TestCtx>> middleware = null;

            Assert.Throws<ArgumentNullException>(() => pipelineBuilder.Use(middleware!));
        }
    }
}