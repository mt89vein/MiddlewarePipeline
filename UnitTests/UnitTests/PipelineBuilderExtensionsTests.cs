using Microsoft.Extensions.DependencyInjection;
using Middlewares;
using NUnit.Framework;
using System;
using System.Threading.Tasks;
using UnitTests.TestMiddlewares;

namespace UnitTests
{
    [TestOf(typeof(PipelineBuilderExtensions))]
    public class PipelineBuilderExtensionsTests
    {
        [Test]
        [TestCase(null, typeof(ArgumentNullException))]
        [TestCase(typeof(TestCtx), typeof(ArgumentException))]
        public void Cannot_Register_InvalidType(Type typeToRegister, Type exceptionType)
        {
            var services = new ServiceCollection();

            var pipelineBuilder = services.ConfigurePipelineFor<TestCtx>();

            Assert.Throws(exceptionType, () => pipelineBuilder.Use(typeToRegister));
        }

        [Test]
        public void Cannot_Register_Invalid_Func()
        {
            var services = new ServiceCollection();

            var pipelineBuilder = services.ConfigurePipelineFor<TestCtx>();

            Func<TestCtx, Func<Task>, Task> middleware = null;

            Assert.Throws<ArgumentNullException>(() => pipelineBuilder.Use(middleware!));
        }

        [Test]
        public void Cannot_Register_Invalid_MiddlewareDelegate()
        {
            var services = new ServiceCollection();

            var pipelineBuilder = services.ConfigurePipelineFor<TestCtx>();

            Func<MiddlewareDelegate<TestCtx>, MiddlewareDelegate<TestCtx>> middleware = null;

            Assert.Throws<ArgumentNullException>(() => pipelineBuilder.Use(middleware!));
        }

        [Test]
        public async Task NoOp_WhenPipeline_Empty()
        {
            var services = new ServiceCollection();

            services.ConfigurePipelineFor<TestCtx>();

            var sp = services.BuildServiceProvider();

            var pipeline = sp.GetRequiredService<IPipeline<TestCtx>>();

            var testCtx = new TestCtx();

            await pipeline.ExecuteAsync(testCtx);

            Assert.IsNull(testCtx.Msg);
        }

        [Test]
        public async Task Executes_With_Register_MiddlewareDelegate()
        {
            var services = new ServiceCollection();

            services
                .ConfigurePipelineFor<TestCtx>()
                .Use(next =>
                {
                    return ctx =>
                    {
                        ctx.Msg = "123";

                        return next(ctx);
                    };
                });

            var sp = services.BuildServiceProvider();

            var pipeline = sp.GetRequiredService<IPipeline<TestCtx>>();

            var testCtx = new TestCtx();

            await pipeline.ExecuteAsync(testCtx);

            Assert.AreEqual("123", testCtx.Msg);
        }
    }
}