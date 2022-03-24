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
        /// Adds a middleware instance to be executed in pipeline.
        /// </summary>
        /// <param name="middleware">Middleware instance.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="middleware"/> is null.</exception>
        /// <returns>A reference to the builder after the operation has completed.</returns>
        IPipelineBuilder<TParameter> Use(IMiddleware<TParameter> middleware);

        /// <summary>
        /// Adds a middleware from factory into pipeline.
        /// </summary>
        /// <param name="middlewareFactory">The middleware factory to be executed.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="middlewareFactory"/> is null.</exception>
        /// <returns>A reference to the builder after the operation has completed.</returns>
        IPipelineBuilder<TParameter> Use(ParameterAsNextMiddlewareFactoryDelegate<TParameter> middlewareFactory);

        /// <summary>
        /// Adds a middleware from factory into pipeline.
        /// </summary>
        /// <param name="middlewareFactory">The middleware factory to be executed.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="middlewareFactory"/> is null.</exception>
        /// <returns>A reference to the builder after the operation has completed.</returns>
        IPipelineBuilder<TParameter> Use(ParameterWithServiceProviderAsNextMiddlewareFactoryDelegate<TParameter> middlewareFactory);

        /// <summary>
        /// Adds a middleware func to be executed in pipeline.
        /// </summary>
        /// <param name="middlewareDelegateWithServiceProvider">The middleware as func to be executed.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="middlewareDelegateWithServiceProvider"/> is null.</exception>
        /// <returns>A reference to the builder after the operation has completed.</returns>
        IPipelineBuilder<TParameter> Use(FuncAsNextMiddlewareDelegateWithServiceProvider<TParameter> middlewareDelegateWithServiceProvider);

        /// <summary>
        /// Adds a middleware func to be executed in pipeline.
        /// </summary>
        /// <param name="middlewareDelegate">The middleware as func to be executed.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="middlewareDelegate"/> is null.</exception>
        /// <returns>A reference to the builder after the operation has completed.</returns>
        IPipelineBuilder<TParameter> Use(FuncAsNextMiddlewareDelegate<TParameter> middlewareDelegate);
    }
}