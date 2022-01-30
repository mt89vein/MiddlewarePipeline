using Microsoft.Extensions.DependencyInjection;
using Middlewares.Tests.TestMiddlewares;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

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

            // act
            void TestCode() => pipelineBuilder.Use((FuncAsNextMiddlewareDelegate<TestCtx>)null);

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

            pipelineBuilder.Use(async (ctx, next, ct) =>
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
    }
}