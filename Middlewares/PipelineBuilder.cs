using System;
using System.Collections.Generic;
using System.Reflection;

namespace Middlewares
{
    /// <summary>
    /// Middleware pipeline builder.
    /// </summary>
    /// <typeparam name="TParameter">Pipeline parameter type.</typeparam>
    public class PipelineBuilder<TParameter> : IPipelineBuilder<TParameter>, IPipelineInfoAccessor<TParameter>
        where TParameter : notnull
    {
        /// <summary>
        /// Pipeline components.
        /// </summary>
        private readonly List<PipelineComponent<TParameter>> _pipelineComponents = new();

        /// <summary>
        /// Pipeline components.
        /// </summary>
        IEnumerable<PipelineComponent<TParameter>> IPipelineInfoAccessor<TParameter>.PipelineComponents => _pipelineComponents;

        /// <summary>
        /// Adds a middleware type to be executed in pipeline.
        /// </summary>
        /// <typeparam name="TMiddleware">Middleware type.</typeparam>
        /// <returns>A reference to the builder after the operation has completed.</returns>
        public IPipelineBuilder<TParameter> Use<TMiddleware>()
            where TMiddleware : IMiddleware<TParameter>
        {
            return Use(typeof(TMiddleware));
        }

        /// <summary>
        /// Adds a middleware type to be executed in pipeline.
        /// </summary>
        /// <param name="middlewareType">The middleware type to be executed.</param>
        /// <exception cref="ArgumentException">Thrown if the <paramref name="middlewareType"/> is
        /// not an implementation of <see cref="IMiddleware{TParameter}"/>.</exception>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="middlewareType"/> is null.</exception>
        /// <returns>A reference to the builder after the operation has completed.</returns>
        public IPipelineBuilder<TParameter> Use(Type middlewareType)
        {
            if (middlewareType is null)
            {
                throw new ArgumentNullException(nameof(middlewareType));
            }

            var isAssignableFromMiddleware = typeof(IMiddleware<TParameter>).GetTypeInfo().IsAssignableFrom(middlewareType.GetTypeInfo());
            if (!isAssignableFromMiddleware)
            {
                throw new ArgumentException(
                    $"The middleware type must implement \"{typeof(IMiddleware<TParameter>)}\"."
                );
            }

            _pipelineComponents.Add(new PipelineComponent<TParameter>(middlewareType));

            return this;
        }

        /// <summary>
        /// Adds a middleware instance to be executed in pipeline.
        /// </summary>
        /// <param name="middleware">Middleware instance.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="middleware"/> is null.</exception>
        /// <returns>A reference to the builder after the operation has completed.</returns>
        public IPipelineBuilder<TParameter> Use(IMiddleware<TParameter> middleware)
        {
            if (middleware is null)
            {
                throw new ArgumentNullException(nameof(middleware));
            }

            _pipelineComponents.Add(new PipelineComponent<TParameter>(middleware));

            return this;
        }

        /// <summary>
        /// Adds a middleware func to be executed in pipeline.
        /// </summary>
        /// <param name="middleware">The middleware as func to be executed.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="middleware"/> is null.</exception>
        /// <returns>A reference to the builder after the operation has completed.</returns>
        public IPipelineBuilder<TParameter> Use(FuncAsNextMiddlewareDelegateWithServiceProvider<TParameter> middleware)
        {
            if (middleware is null)
            {
                throw new ArgumentNullException(nameof(middleware));
            }

            _pipelineComponents.Add(new PipelineComponent<TParameter>(middleware));

            return this;
        }

        /// <summary>
        /// Adds a middleware func to be executed in pipeline.
        /// </summary>
        /// <param name="middlewareDelegate">The middleware as func to be executed.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="middlewareDelegate"/> is null.</exception>
        /// <returns>A reference to the builder after the operation has completed.</returns>
        public IPipelineBuilder<TParameter> Use(FuncAsNextMiddlewareDelegate<TParameter> middlewareDelegate)
        {
            if (middlewareDelegate is null)
            {
                throw new ArgumentNullException(nameof(middlewareDelegate));
            }

            _pipelineComponents.Add(new PipelineComponent<TParameter>(middlewareDelegate));

            return this;
        }

        /// <summary>
        /// Adds a middleware from factory into pipeline.
        /// </summary>
        /// <param name="middlewareFactory">The middleware factory to be executed.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="middlewareFactory"/> is null.</exception>
        /// <returns>A reference to the builder after the operation has completed.</returns>
        public IPipelineBuilder<TParameter> Use(ParameterAsNextMiddlewareFactoryDelegate<TParameter> middlewareFactory)
        {
            if (middlewareFactory is null)
            {
                throw new ArgumentNullException(nameof(middlewareFactory));
            }

            _pipelineComponents.Add(new PipelineComponent<TParameter>(middlewareFactory));

            return this;
        }

        /// <summary>
        /// Adds a middleware from factory into pipeline.
        /// </summary>
        /// <param name="middlewareFactory">The middleware factory to be executed.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="middlewareFactory"/> is null.</exception>
        /// <returns>A reference to the builder after the operation has completed.</returns>
        public IPipelineBuilder<TParameter> Use(ParameterWithServiceProviderAsNextMiddlewareFactoryDelegate<TParameter> middlewareFactory)
        {
            if (middlewareFactory is null)
            {
                throw new ArgumentNullException(nameof(middlewareFactory));
            }

            _pipelineComponents.Add(new PipelineComponent<TParameter>(middlewareFactory));

            return this;
        }

        /// <summary>
        /// Creates <see cref="IPipeline{TParameter}"/> from current pipeline components.
        /// </summary>
        /// <param name="serviceProvider">Application service provider.</param>
        /// <returns>Pipeline.</returns>
        public IPipeline<TParameter> Build(IServiceProvider? serviceProvider = null)
        {
            return new Pipeline<TParameter>(serviceProvider, _pipelineComponents);
        }
    }
}