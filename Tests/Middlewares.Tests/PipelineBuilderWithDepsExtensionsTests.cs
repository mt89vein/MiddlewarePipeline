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
    [TestOf(typeof(PipelineBuilderWithDepsExtensions))]
    public class PipelineBuilderWithDepsExtensionsTests
    {
        /// <summary>
        /// Null checks.
        /// </summary>
        [Test]
        public void Should_Throw_ArgumentNullException_When_Invalid_Func_WithSingleDep_Provided()
        {
            // arrange
            var services = new ServiceCollection();

            var pipelineBuilder = services.ConfigurePipelineFor<TestCtx>();

            Func<TestCtx, ExampleDependency, Func<Task>, CancellationToken, Task> middleware = null;

            // act
            void TestCode() => pipelineBuilder.Use(middleware!);

            // assert
            Assert.Throws<ArgumentNullException>(TestCode);
        }

        /// <summary>
        /// Null checks.
        /// </summary>
        [Test]
        public void Should_Throw_ArgumentNullException_When_Invalid_Func_WithThreeDeps_Provided()
        {
            // arrange
            var services = new ServiceCollection();

            var pipelineBuilder = services.ConfigurePipelineFor<TestCtx>();

            Func<TestCtx, ExampleDependency, ExampleDependency, ExampleDependency, Func<Task>, CancellationToken, Task> middleware = null;

            // act
            void TestCode() => pipelineBuilder.Use(middleware!);

            // assert
            Assert.Throws<ArgumentNullException>(TestCode);
        }

        /// <summary>
        /// Null checks.
        /// </summary>
        [Test]
        public void Should_Throw_ArgumentNullException_When_Invalid_Func_WithTwoDeps_Provided()
        {
            // arrange
            var services = new ServiceCollection();

            var pipelineBuilder = services.ConfigurePipelineFor<TestCtx>();

            Func<TestCtx, ExampleDependency, ExampleDependency, Func<Task>, CancellationToken, Task> middleware = null;

            // act
            void TestCode() => pipelineBuilder.Use(middleware!);

            // assert
            Assert.Throws<ArgumentNullException>(TestCode);
        }

        /// <summary>
        /// Ensures, that we resolve and pass to pipeline requested deps.
        /// </summary>
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

            pipelineBuilder.Use<TestCtx, ExampleDependency>((_, dependency, next, _) =>
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

        /// <summary>
        /// Ensures, that we resolve and pass to pipeline requested deps.
        /// </summary>
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

            pipelineBuilder.Use<TestCtx, ExampleDependency, ExampleDependency>((_, dependency1, dependency2, next, _) =>
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

        /// <summary>
        /// Ensures, that we resolve and pass to pipeline requested deps.
        /// </summary>
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

            pipelineBuilder.Use<TestCtx, ExampleDependency, ExampleDependency, ExampleDependency>((_, dependency1, dependency2, dependency3, next, _) =>
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