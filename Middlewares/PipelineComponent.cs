using System;

namespace Middlewares
{
    /// <summary>
    /// Pipeline component.
    /// </summary>
    /// <typeparam name="TParameter">Pipeline parameter type.</typeparam>
    internal sealed class PipelineComponent<TParameter>
        where TParameter : notnull
    {
        /// <summary>
        /// Next middleware type.
        /// </summary>
        public Type? NextMiddlewareType { get; }

        /// <summary>
        /// Next middleware instance.
        /// </summary>
        public IMiddleware<TParameter>? NextMiddleware { get; }

        /// <summary>
        /// Next middleware factory with service provider.
        /// </summary>
        public ParameterWithServiceProviderAsNextMiddlewareFactoryDelegate<TParameter>? NextMiddlewareWithProviderFactory { get; }

        /// <summary>
        /// Next middleware factory.
        /// </summary>
        public ParameterAsNextMiddlewareFactoryDelegate<TParameter>? NextMiddlewareFactory { get; }

        /// <summary>
        /// Next middleware func with service provider.
        /// </summary>
        public FuncAsNextMiddlewareDelegateWithServiceProvider<TParameter>? NextFuncWithServiceProvider { get; }

        /// <summary>
        /// Next middleware func.
        /// </summary>
        public FuncAsNextMiddlewareDelegate<TParameter>? NextFunc { get; }

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
        /// <param name="middleware">Next middleware instance.</param>
        public PipelineComponent(IMiddleware<TParameter> middleware)
        {
            NextMiddleware = middleware ?? throw new ArgumentNullException(nameof(middleware));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PipelineComponent{TParameter}"></see> class.
        /// </summary>
        /// <param name="middlewareFactory">Next middleware factory.</param>
        public PipelineComponent(ParameterAsNextMiddlewareFactoryDelegate<TParameter> middlewareFactory)
        {
            NextMiddlewareFactory = middlewareFactory ?? throw new ArgumentNullException(nameof(middlewareFactory));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PipelineComponent{TParameter}"></see> class.
        /// </summary>
        /// <param name="middlewareFactory">Next middleware factory.</param>
        public PipelineComponent(ParameterWithServiceProviderAsNextMiddlewareFactoryDelegate<TParameter> middlewareFactory)
        {
            NextMiddlewareWithProviderFactory = middlewareFactory ?? throw new ArgumentNullException(nameof(middlewareFactory));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PipelineComponent{TParameter}"></see> class.
        /// </summary>
        /// <param name="nextFuncWithServiceProvider">Next middleware func.</param>
        public PipelineComponent(FuncAsNextMiddlewareDelegateWithServiceProvider<TParameter> nextFuncWithServiceProvider)
        {
            NextFuncWithServiceProvider = nextFuncWithServiceProvider ?? throw new ArgumentNullException(nameof(nextFuncWithServiceProvider));
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="PipelineComponent{TParameter}"></see> class.
        /// </summary>
        /// <param name="nextFunc">Next middleware func.</param>
        public PipelineComponent(FuncAsNextMiddlewareDelegate<TParameter> nextFunc)
        {
            NextFunc = nextFunc ?? throw new ArgumentNullException(nameof(nextFunc));
        }

        /// <summary>
        /// Returns true, only if service provider is not required for this pipeline.
        /// </summary>
        internal bool CanExecuteWithoutServiceProvider()
        {
            return NextMiddleware is not null || NextMiddlewareFactory is not null || NextFunc is not null;
        }
        
        /// <summary>
        /// Returns true, only if component have correct executable middleware.
        /// </summary>
        internal bool IsValidComponent()
        {
            return NextMiddleware is not null ||
                   NextMiddlewareType is not null ||
                   NextMiddlewareFactory is not null ||
                   NextMiddlewareWithProviderFactory is not null ||
                   NextFuncWithServiceProvider is not null ||
                   NextFunc is not null;
        }
    }
}