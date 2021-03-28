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
        Task InvokeAsync(TParameter parameter, NextMiddleware next);
    }

    /// <summary>
    /// Delegate as Middleware.
    /// </summary>
    public delegate Task MiddlewareDelegate<TParameter>(TParameter parameter);

    /// <summary>
    /// Pipeline component.
    /// </summary>
    public delegate Task NextMiddleware();
}