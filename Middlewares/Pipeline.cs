using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Middlewares
{
    /// <summary>
    /// Pipeline for <typeparamref name="TParameter"/>.
    /// </summary>
    /// <typeparam name="TParameter">Pipeline parameter type.</typeparam>
    public class Pipeline<TParameter> : IPipeline<TParameter>
        where TParameter : class
    {
        /// <summary>
        /// Application service provider.
        /// </summary>
        private readonly IServiceProvider _serviceProvider;

        /// <summary>
        /// Pipeline components.
        /// </summary>
        private readonly IList<PipelineComponents<TParameter>> _pipelineComponents;

        /// <summary>
        /// Completed middleware.
        /// </summary>
        private readonly NextMiddleware _completedMiddleware = () => Task.CompletedTask;

        /// <summary>
        /// Initiates new instance of <see cref="Pipeline{TParameter}"/> class.
        /// </summary>
        /// <param name="serviceProvider">Application service provider.</param>
        /// <param name="pipelineInfoAccessor">Pipeline information accessor.</param>
        public Pipeline(IServiceProvider serviceProvider, IPipelineInfoAccessor<TParameter> pipelineInfoAccessor)
        {
            _serviceProvider = serviceProvider;
            _pipelineComponents = new List<PipelineComponents<TParameter>>(pipelineInfoAccessor.PipelineComponents);
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
            NextMiddleware next = _completedMiddleware;

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
                    var typedMiddleware = (IMiddleware<TParameter>)ActivatorUtilities.CreateInstance(_serviceProvider, type.NextMiddlewareType);

                    return typedMiddleware.InvokeAsync(parameter, next, cancellationToken);
                }

                if (type.NextFunc is not null)
                {
                    return type.NextFunc(_serviceProvider, (_, _) => next())(parameter, cancellationToken);
                }

                throw new InvalidOperationException("Invalid pipeline component. No middleware or delegate supplied.");
            };

            return next();
        }
    }
}