using System;
using System.Threading;
using System.Threading.Tasks;

namespace Middlewares
{
    /// <summary>
    /// Extension methods for <see cref="IPipelineBuilder{TParameter}"/>.
    /// </summary>
    public static class PipelineBuilderExtensions
    {
        /// <summary>
        /// Adds a middleware func to be executed in pipeline.
        /// </summary>
        /// <param name="builder">Pipeline builder.</param>
        /// <param name="middleware">The middleware as func to be executed.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="middleware"/> is null.</exception>
        /// <returns>A reference to the builder after the operation has completed.</returns>
        public static IPipelineBuilder<TParameter> Use<TParameter>(
            this IPipelineBuilder<TParameter> builder,
            Func<TParameter, NextMiddleware, CancellationToken, Task> middleware
        )
            where TParameter : class
        {
            if (middleware is null)
            {
                throw new ArgumentNullException(nameof(middleware));
            }

            return builder.Use((_, next) => (context, cancellationToken) =>
            {
                return middleware(context, () => next(context, cancellationToken), cancellationToken);
            });
        }

        /// <summary>
        /// Adds a middleware func to be executed in pipeline.
        /// </summary>
        /// <param name="builder">Pipeline builder.</param>
        /// <param name="middleware">The middleware as func to be executed.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="middleware"/> is null.</exception>
        /// <returns>A reference to the builder after the operation has completed.</returns>
        public static IPipelineBuilder<TParameter> Use<TParameter>(
            this IPipelineBuilder<TParameter> builder,
            Func<MiddlewareDelegate<TParameter>, MiddlewareDelegate<TParameter>> middleware
        )
        {
            if (middleware is null)
            {
                throw new ArgumentNullException(nameof(middleware));
            }

            return builder.Use((_, next) => (context, cancellationToken) =>
            {
                return middleware((c, _) => next(c, cancellationToken))(context, cancellationToken);
            });
        }
    }
}