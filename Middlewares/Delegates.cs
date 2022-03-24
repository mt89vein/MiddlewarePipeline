using System;
using System.Threading;
using System.Threading.Tasks;

namespace Middlewares
{
    /// <summary>
    /// Delegate as Middleware.
    /// </summary>
    public delegate Task MiddlewareDelegate<TParameter>(TParameter parameter, CancellationToken cancellationToken = default);

    /// <summary>
    /// Pipeline component.
    /// </summary>
    public delegate Task NextMiddleware();

    /// <summary>
    /// Next middleware factory delegate with service provider.
    /// </summary>
    /// <typeparam name="TParameter">Pipeline parameter type.</typeparam>
    /// <param name="serviceProvider">DI Service provider.</param>
    /// <param name="parameter">Pipeline parameter.</param>
    /// <returns>Middleware.</returns>
    public delegate IMiddleware<TParameter> ParameterWithServiceProviderAsNextMiddlewareFactoryDelegate<TParameter>(
        IServiceProvider serviceProvider,
        TParameter parameter
    );

    /// <summary>
    /// Next middleware factory delegate with service provider.
    /// </summary>
    /// <typeparam name="TParameter">Pipeline parameter type.</typeparam>
    /// <param name="parameter">Pipeline parameter.</param>
    /// <returns>Middleware.</returns>
    public delegate IMiddleware<TParameter> ParameterAsNextMiddlewareFactoryDelegate<TParameter>(TParameter parameter);

    /// <summary>
    /// Next middleware factory delegate with service provider.
    /// </summary>
    /// <typeparam name="TParameter">Pipeline parameter type.</typeparam>
    /// <param name="serviceProvider">DI Service provider.</param>
    /// <param name="nextMiddleware">Middleware function.</param>
    /// <returns>Middleware func.</returns>
    public delegate MiddlewareDelegate<TParameter> FuncAsNextMiddlewareDelegateWithServiceProvider<TParameter>(
        IServiceProvider serviceProvider,
        MiddlewareDelegate<TParameter> nextMiddleware
    );

    /// <summary>
    /// Next middleware factory delegate.
    /// </summary>
    /// <typeparam name="TParameter">Pipeline parameter type.</typeparam>
    /// <param name="nextMiddleware">Middleware function.</param>
    /// <returns>Middleware func.</returns>
    public delegate MiddlewareDelegate<TParameter> FuncAsNextMiddlewareDelegate<TParameter>(MiddlewareDelegate<TParameter> nextMiddleware);
}
