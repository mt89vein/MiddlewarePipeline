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
        where TParameter : class
    {
        /// <summary>
        /// Pipeline components.
        /// </summary>
        private readonly List<PipelineComponents<TParameter>> _pipelineComponents = new();

        /// <summary>
        /// Pipeline components.
        /// </summary>
        public IEnumerable<PipelineComponents<TParameter>> PipelineComponents => _pipelineComponents;

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
            if (middlewareType == null)
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

            _pipelineComponents.Add(new PipelineComponents<TParameter>(middlewareType));

            return this;
        }

        /// <summary>
        /// Adds a middleware func to be executed in pipeline.
        /// </summary>
        /// <param name="middleware">The middleware as func to be executed.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="middleware"/> is null.</exception>
        /// <returns>A reference to the builder after the operation has completed.</returns>
        public IPipelineBuilder<TParameter> Use(Func<IServiceProvider, MiddlewareDelegate<TParameter>, MiddlewareDelegate<TParameter>> middleware)
        {
            if (middleware == null)
            {
                throw new ArgumentNullException(nameof(middleware));
            }

            _pipelineComponents.Add(new PipelineComponents<TParameter>(middleware));

            return this;
        }
    }
}