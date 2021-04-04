using System;

namespace Middlewares
{
    /// <summary>
    /// Pipeline component.
    /// </summary>
    /// <typeparam name="TParameter">Pipeline parameter type.</typeparam>
    public class PipelineComponent<TParameter>
        where TParameter : class
    {
        /// <summary>
        /// Next middleware func.
        /// </summary>
        public Func<IServiceProvider, MiddlewareDelegate<TParameter>, MiddlewareDelegate<TParameter>>? NextFunc { get; }

        /// <summary>
        /// Next middleware type.
        /// </summary>
        public Type? NextMiddlewareType { get; }

        /// <summary>
        /// Internal ctor for unit testing.
        /// </summary>
        internal PipelineComponent() {}

        /// <summary>
        /// Initializes a new instance of the <see cref="PipelineComponent{TParameter}"></see> class.
        /// </summary>
        /// <param name="nextMiddlewareType">Next middleware.</param>
        public PipelineComponent(Type nextMiddlewareType)
        {
            NextMiddlewareType = nextMiddlewareType ?? throw new ArgumentNullException(nameof(nextMiddlewareType));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PipelineComponent{TParameter}"></see> class.
        /// </summary>
        /// <param name="nextFunc">Next middleware func.</param>
        public PipelineComponent(Func<IServiceProvider, MiddlewareDelegate<TParameter>, MiddlewareDelegate<TParameter>> nextFunc)
        {
            NextFunc = nextFunc ?? throw new ArgumentNullException(nameof(nextFunc));
        }
    }
}