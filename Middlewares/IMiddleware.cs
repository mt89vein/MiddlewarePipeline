using System.Threading;
using System.Threading.Tasks;

namespace Middlewares
{
    /// <summary>
    /// Middleware.
    /// </summary>
    /// <typeparam name="TParameter">Pipeline parameter type.</typeparam>
    public interface IMiddleware<TParameter>
    {
        /// <summary>
        /// Invokes middleware.
        /// </summary>
        /// <param name="parameter">Pipeline parameter.</param>
        /// <param name="next">Next middleware.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        Task InvokeAsync(TParameter parameter, NextMiddleware next, CancellationToken cancellationToken = default);
    }
}