using System.Collections.Generic;

namespace Middlewares
{
    /// <summary>
    /// Pipeline information accessor.
    /// </summary>
    /// <typeparam name="TParameter">Pipeline parameter type.</typeparam>
    public interface IPipelineInfoAccessor<TParameter>
        where TParameter : class
    {
        /// <summary>
        /// Pipeline components.
        /// </summary>
        IEnumerable<PipelineComponent<TParameter>> PipelineComponents { get; }
    }
}