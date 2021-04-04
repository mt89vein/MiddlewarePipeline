using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace Middlewares
{
    /// <summary>
    /// Extension methods for <see cref="IPipelineBuilder{TParameter}"/>.
    /// </summary>
    public static class PipelineBuilderConditionalExtensions
    {
        /// <summary>
        /// Adds a middleware func to be executed in pipeline.
        /// </summary>
        /// <param name="builder">Pipeline builder.</param>
        /// <param name="predicate">If returns true, pipeline will be executed.</param>
        /// <returns>A reference to the builder after the operation has completed.</returns>
        public static IPipelineBuilder<TParameter> UseWhen<TParameter, TMiddleware>(
            this IPipelineBuilder<TParameter> builder,
            Func<TParameter, bool> predicate
        )
            where TParameter : class
            where TMiddleware : IMiddleware<TParameter>
        {
            return builder.Use((sp, next) => (context, cancellationToken) =>
            {
                if (predicate(context))
                {
                    var middleware = (IMiddleware<TParameter>)ActivatorUtilities.CreateInstance<TMiddleware>(sp);

                    return middleware.InvokeAsync(context, () => next(context, cancellationToken), cancellationToken);
                }

                return next(context, cancellationToken);
            });
        }

        /// <summary>
        /// Adds a middleware func to be executed in pipeline.
        /// </summary>
        /// <param name="builder">Pipeline builder.</param>
        /// <param name="predicate">If returns true, pipeline will be executed.</param>
        /// <param name="middleware">Middleware to add.</param>
        /// <returns>A reference to the builder after the operation has completed.</returns>
        public static IPipelineBuilder<TParameter> UseWhen<TParameter>(
            this IPipelineBuilder<TParameter> builder,
            Func<TParameter, bool> predicate,
            Func<IServiceProvider, TParameter, Func<Task>, Task> middleware
        )
            where TParameter : class
        {
            if (middleware == null)
            {
                throw new ArgumentNullException(nameof(middleware));
            }

            return builder.Use((sp, next) => (context, cancellationToken) =>
            {
                if (predicate(context))
                {
                    return middleware(sp, context, () => next(context, cancellationToken));
                }

                return next(context, cancellationToken);
            });
        }
    }
}