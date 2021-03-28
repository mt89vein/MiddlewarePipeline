using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace Middlewares
{
    /// <summary>
    /// Extension methods for <see cref="IPipelineBuilder{TParameter}"/>.
    /// </summary>
    public static class PipelineBuilderWithDepsExtensions
    {
        /// <summary>
        /// Adds a middleware func to be executed in pipeline.
        /// </summary>
        /// <param name="builder">Pipeline builder.</param>
        /// <param name="middleware">The middleware as func to be executed.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="middleware"/> is null.</exception>
        /// <returns>A reference to the builder after the operation has completed.</returns>
        public static IPipelineBuilder<TParameter> Use<TParameter, TDep1>(
            this IPipelineBuilder<TParameter> builder,
            Func<TParameter, TDep1, Func<Task>, Task> middleware
        )
            where TParameter : class
            where TDep1 : notnull
        {
            if (middleware == null)
            {
                throw new ArgumentNullException(nameof(middleware));
            }

            return builder.Use((sp, next) => context =>
            {
                var dep1 = sp.GetRequiredService<TDep1>();

                return middleware(context, dep1, () => next(context));
            });
        }

        /// <summary>
        /// Adds a middleware func to be executed in pipeline.
        /// </summary>
        /// <param name="builder">Pipeline builder.</param>
        /// <param name="middleware">The middleware as func to be executed.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="middleware"/> is null.</exception>
        /// <returns>A reference to the builder after the operation has completed.</returns>
        public static IPipelineBuilder<TParameter> Use<TParameter, TDep1, TDep2>(
            this IPipelineBuilder<TParameter> builder,
            Func<TParameter, TDep1, TDep2, Func<Task>, Task> middleware
        )
            where TParameter : class
            where TDep1 : notnull
            where TDep2 : notnull
        {
            if (middleware == null)
            {
                throw new ArgumentNullException(nameof(middleware));
            }

            return builder.Use((sp, next) => context =>
            {
                var dep1 = sp.GetRequiredService<TDep1>();
                var dep2 = sp.GetRequiredService<TDep2>();

                return middleware(context, dep1, dep2, () => next(context));
            });
        }

        /// <summary>
        /// Adds a middleware func to be executed in pipeline.
        /// </summary>
        /// <param name="builder">Pipeline builder.</param>
        /// <param name="middleware">The middleware as func to be executed.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="middleware"/> is null.</exception>
        /// <returns>A reference to the builder after the operation has completed.</returns>
        public static IPipelineBuilder<TParameter> Use<TParameter, TDep1, TDep2, TDep3>(
            this IPipelineBuilder<TParameter> builder,
            Func<TParameter, TDep1, TDep2, TDep3, Func<Task>, Task> middleware
        )
            where TParameter : class
            where TDep1 : notnull
            where TDep2 : notnull
            where TDep3 : notnull
        {
            if (middleware == null)
            {
                throw new ArgumentNullException(nameof(middleware));
            }

            return builder.Use((sp, next) => context =>
            {
                var dep1 = sp.GetRequiredService<TDep1>();
                var dep2 = sp.GetRequiredService<TDep2>();
                var dep3 = sp.GetRequiredService<TDep3>();

                return middleware(context, dep1, dep2, dep3, () => next(context));
            });
        }
    }
}