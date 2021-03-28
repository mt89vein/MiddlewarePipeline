using System;

namespace Middlewares
{
    /// <summary>
    /// Pipeline component.
    /// </summary>
    /// <typeparam name="TParameter">Pipeline parameter type.</typeparam>
    public class PipelineComponents<TParameter>
        where TParameter : class
    {
        /// <summary>
        /// Next middleware func.
        /// </summary>
        public Func<IServiceProvider, MiddlewareDelegate<TParameter>, MiddlewareDelegate<TParameter>>? NextFunc { get; set; }

        /// <summary>
        /// Next middleware type.
        /// </summary>
        public Type? NextMiddlewareType { get; set; }
    }
}