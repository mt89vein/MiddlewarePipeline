using Microsoft.Extensions.DependencyInjection;
using Middlewares.Tests.TestMiddlewares;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace Middlewares.Tests
{
    [TestOf(typeof(PipelineBuilderWithDepsExtensions))]
    public class PipelineBuilderWithDepsExtensionsTests
    {
        [Test]
        public void Cannot_Register_Invalid_Func_WithSingleDep()
        {
            // arrange
            var services = new ServiceCollection();

            var pipelineBuilder = services.ConfigurePipelineFor<TestCtx>();

            Func<TestCtx, ExampleDependency, Func<Task>, Task> middleware = null;

            // act
            void TestCode() => pipelineBuilder.Use(middleware!);

            // assert
            Assert.Throws<ArgumentNullException>(TestCode);
        }

        [Test]
        public async Task Correctly_Resolves_SingleDep()
        {
            // arrange
            var dep = new ExampleDependency
            {
                Resolved = false
            };

            var services = new ServiceCollection();
            services.AddSingleton(dep);

            var pipelineBuilder = services.ConfigurePipelineFor<TestCtx>();

            pipelineBuilder.Use<TestCtx, ExampleDependency>((_, dependency, next) =>
            {
                dependency.Resolved = true;

                return next();
            });

            var sp = services.BuildServiceProvider();

            var pipeline = sp.GetRequiredService<IPipeline<TestCtx>>();

            // act
            await pipeline.ExecuteAsync(new TestCtx());

            // assert
            Assert.IsTrue(dep.Resolved, "Dependency flag resolved is not true");
        }

        [Test]
        public void Cannot_Register_Invalid_Func_WithTwoDeps()
        {
            // arrange
            var services = new ServiceCollection();

            var pipelineBuilder = services.ConfigurePipelineFor<TestCtx>();

            Func<TestCtx, ExampleDependency, ExampleDependency, Func<Task>, Task> middleware = null;

            // act
            void TestCode() => pipelineBuilder.Use(middleware!);

            // assert
            Assert.Throws<ArgumentNullException>(TestCode);
        }

        [Test]
        public async Task Correctly_Resolves_TwoDeps()
        {
            // arrange
            var dep = new ExampleDependency
            {
                Resolved = false
            };

            var services = new ServiceCollection();
            services.AddSingleton(dep);

            var pipelineBuilder = services.ConfigurePipelineFor<TestCtx>();

            pipelineBuilder.Use<TestCtx, ExampleDependency, ExampleDependency>((_, dependency1, dependency2, next) =>
            {
                dependency1.Resolved = true;

                Assert.AreSame(dependency1, dependency2);

                return next();
            });

            var sp = services.BuildServiceProvider();

            var pipeline = sp.GetRequiredService<IPipeline<TestCtx>>();

            // act
            await pipeline.ExecuteAsync(new TestCtx());

            // assert
            Assert.IsTrue(dep.Resolved, "Dependency flag resolved is not true");
        }

        [Test]
        public void Cannot_Register_Invalid_Func_WithThreeDeps()
        {
            // arrange
            var services = new ServiceCollection();

            var pipelineBuilder = services.ConfigurePipelineFor<TestCtx>();

            Func<TestCtx, ExampleDependency, ExampleDependency, ExampleDependency, Func<Task>, Task> middleware = null;

            // act
            void TestCode() => pipelineBuilder.Use(middleware!);

            // assert
            Assert.Throws<ArgumentNullException>(TestCode);
        }

        [Test]
        public async Task Correctly_Resolves_ThreeDeps()
        {
            // arrange
            var dep = new ExampleDependency
            {
                Resolved = false
            };

            var services = new ServiceCollection();
            services.AddSingleton(dep);

            var pipelineBuilder = services.ConfigurePipelineFor<TestCtx>();

            pipelineBuilder.Use<TestCtx, ExampleDependency, ExampleDependency, ExampleDependency>((_, dependency1, dependency2, dependency3, next) =>
            {
                dependency1.Resolved = true;

                Assert.AreSame(dependency1, dependency2);
                Assert.AreSame(dependency2, dependency3);

                return next();
            });

            var sp = services.BuildServiceProvider();

            var pipeline = sp.GetRequiredService<IPipeline<TestCtx>>();

            // act
            await pipeline.ExecuteAsync(new TestCtx());

            // assert
            Assert.IsTrue(dep.Resolved, "Dependency flag resolved is not true");
        }
    }
}