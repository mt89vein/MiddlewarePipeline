using Microsoft.Extensions.DependencyInjection;
using Middlewares;
using NUnit.Framework;
using System.Threading.Tasks;
using UnitTests.TestMiddlewares;

namespace UnitTests
{
    [TestOf(typeof(ServiceCollectionExtensions))]
    public class ConfigurePipelineForTests
    {
        /// <summary>
        /// Pipeline execution via service provider.
        /// </summary>
        [Test]
        public async Task ConfigurePipelineFor_WithCorrectExecution()
        {
            // arrange
            var services = new ServiceCollection();
            services.ConfigurePipelineFor<TestCtx>()
                .Use<Middleware1>()
                .Use(typeof(Middleware2));

            var sp = services.BuildServiceProvider();
            var pipeline = sp.GetRequiredService<IPipeline<TestCtx>>();

            var testContext = new TestCtx();

            Assert.IsNull(testContext.Msg);
            Assert.Zero(testContext.ExecutedMiddlewaresCount);

            // act
            await pipeline.ExecuteAsync(testContext);

            // assert
            const string expectedMessage = "Before_" + nameof(Middleware1) +
                                           "Before_" + nameof(Middleware2) +
                                           "After_" + nameof(Middleware2) +
                                           "After_" + nameof(Middleware1);

            Assert.AreEqual(expectedMessage, testContext.Msg, "Pipeline execution order is not match");
            Assert.AreEqual(2, testContext.ExecutedMiddlewaresCount, "ExecutedMiddlewaresCount is not match");
        }

        [Test]
        public void PipelineAccessor_CanBeResolved()
        {
            var services = new ServiceCollection();
            services.ConfigurePipelineFor<TestCtx>();

            var sp = services.BuildServiceProvider();

            var pipelineInfoAccessor = sp.GetService<IPipelineInfoAccessor<TestCtx>>();

            Assert.IsNotNull(pipelineInfoAccessor, "Pipeline accessor could not be resolved");
        }
    }
}