using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Middlewares
{
    /// <summary>
    /// Pipeline for <typeparamref name="TParameter"/>.
    /// </summary>
    /// <typeparam name="TParameter">Pipeline parameter type.</typeparam>
    internal class Pipeline<TParameter> : IPipeline<TParameter>
        where TParameter : class
    {
        /// <summary>
        /// Application service provider.
        /// </summary>
        private readonly IServiceProvider _serviceProvider;

        /// <summary>
        /// Pipeline components.
        /// </summary>
        private readonly IList<PipelineComponent<TParameter>> _pipelineComponents;

        /// <summary>
        /// Completed middleware.
        /// </summary>
        private readonly NextMiddleware _completedMiddleware = () => Task.CompletedTask;

        /// <summary>
        /// Initiates new instance of <see cref="Pipeline{TParameter}"/> class.
        /// </summary>
        /// <param name="serviceProvider">Application service provider.</param>
        /// <param name="pipelineInfoAccessor">Pipeline information accessor.</param>
        /// <exception cref="ArgumentNullException">If ServiceProvider required.</exception>
        public Pipeline(IServiceProvider? serviceProvider, IPipelineInfoAccessor<TParameter> pipelineInfoAccessor)
            : this(serviceProvider, new List<PipelineComponent<TParameter>>(pipelineInfoAccessor.PipelineComponents))
        {
        }

        /// <summary>
        /// Initiates new instance of <see cref="Pipeline{TParameter}"/> class.
        /// </summary>
        /// <param name="serviceProvider">Application service provider.</param>
        /// <param name="pipelineComponents">Pipeline components (middlewares).</param>
        /// <exception cref="ArgumentNullException">If ServiceProvider required.</exception>
        internal Pipeline(IServiceProvider? serviceProvider, IEnumerable<PipelineComponent<TParameter>> pipelineComponents)
        {
            _pipelineComponents = new List<PipelineComponent<TParameter>>(pipelineComponents);
            _serviceProvider = serviceProvider!;

            if (!_pipelineComponents.All(x => x.IsValidComponent()))
            {
                throw new ArgumentException("Invalid pipeline component detected. No middleware or delegate supplied.");
            }

            if (serviceProvider is null && !_pipelineComponents.All(x => x.CanExecuteWithoutServiceProvider()))
            {
                throw new ArgumentNullException(nameof(serviceProvider),
                    "When using non DI builder, you should provide middleware pipeline instances by yourself or from factory without service provider.");
            }
        }

        /// <summary>
        /// Execute configured pipeline.
        /// </summary>
        /// <param name="parameter">Pipeline parameter.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        public Task ExecuteAsync(TParameter parameter, CancellationToken cancellationToken = default)
        {
            if (_pipelineComponents.Count == 0)
            {
                return Task.CompletedTask;
            }

            var index = 0;
            var next = _completedMiddleware;

            next = () =>
            {
                var type = _pipelineComponents[index];

                index++;
                if (index == _pipelineComponents.Count)
                {
                    next = _completedMiddleware; // final action
                }

                if (type.NextMiddlewareType is not null)
                {
                    var typedMiddleware = (IMiddleware<TParameter>)ActivatorUtilities.GetServiceOrCreateInstance(_serviceProvider, type.NextMiddlewareType);

                    return typedMiddleware.InvokeAsync(parameter, next, cancellationToken);
                }

                if (type.NextFunc is not null)
                {
                    return type.NextFunc(_serviceProvider, (_, _) => next())(parameter, cancellationToken);
                }

                if (type.NextMiddlewareFactory is not null)
                {
                    return type.NextMiddlewareFactory(parameter).InvokeAsync(parameter, next, cancellationToken);
                }

                if (type.NextMiddlewareWithProviderFactory is not null)
                {
                    return type.NextMiddlewareWithProviderFactory(_serviceProvider, parameter).InvokeAsync(parameter, next, cancellationToken);
                }

                if (type.NextMiddleware is not null)
                {
                    return type.NextMiddleware.InvokeAsync(parameter, next, cancellationToken);
                }

                throw new InvalidOperationException("Invalid pipeline component. No middleware or delegate supplied.");
            };

            return next();
        }
    }
}