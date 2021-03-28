using System;

namespace Middlewares
{
    /// <summary>
    /// Middleware pipeline builder.
    /// </summary>
    /// <typeparam name="TParameter">Parameter type.</typeparam>
    public interface IPipelineBuilder<TParameter>
    {
        /// <summary>
        /// Adds a middleware type to be executed in pipeline.
        /// </summary>
        /// <typeparam name="TMiddleware">Middleware type.</typeparam>
        /// <returns>A reference to the builder after the operation has completed.</returns>
        IPipelineBuilder<TParameter> Use<TMiddleware>()
            where TMiddleware : IMiddleware<TParameter>;

        /// <summary>
        /// Adds a middleware type to be executed in pipeline.
        /// </summary>
        /// <param name="middlewareType">The middleware type to be executed.</param>
        /// <exception cref="ArgumentException">Thrown if the <paramref name="middlewareType"/> is
        /// not an implementation of <see cref="IMiddleware{TParameter}"/>.</exception>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="middlewareType"/> is null.</exception>
        /// <returns>A reference to the builder after the operation has completed.</returns>
        IPipelineBuilder<TParameter> Use(Type middlewareType);

        /// <summary>
        /// Adds a middleware func to be executed in pipeline.
        /// </summary>
        /// <param name="middleware">The middleware as func to be executed.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="middleware"/> is null.</exception>
        /// <returns>A reference to the builder after the operation has completed.</returns>
        IPipelineBuilder<TParameter> Use(Func<IServiceProvider, MiddlewareDelegate<TParameter>, MiddlewareDelegate<TParameter>> middleware);
    }
}