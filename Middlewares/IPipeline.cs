using System.Threading;
using System.Threading.Tasks;

namespace Middlewares
{
    /// <summary>
    /// Pipeline for <typeparamref name="TParameter"/>.
    /// </summary>
    /// <typeparam name="TParameter">Pipeline parameter type.</typeparam>
    public interface IPipeline<TParameter>
    {
        /// <summary>
        /// Execute configured pipeline.
        /// </summary>
        /// <param name="parameter">Pipeline parameter.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        Task ExecuteAsync(TParameter parameter, CancellationToken cancellationToken = default);
    }
}