using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Middlewares
{
    /// <summary>
    /// Pipeline for <typeparamref name="TParameter"/>.
    /// </summary>
    /// <typeparam name="TParameter">Pipeline parametr type.</typeparam>
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
        /// Initiates new instance of <see cref="Pipeline{TParameter}"/>.
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
        public Task ExecuteAsync(TParameter parameter)
        {
            if (_pipelineComponents.Count == 0)
            {
                return Task.CompletedTask;
            }

            var index = 0;
            NextMiddleware action = null!;

            action = () =>
            {
                var type = _pipelineComponents[index];

                index++;
                if (index == _pipelineComponents.Count)
                {
                    action = () => Task.CompletedTask; // final action
                }

                if (type.NextMiddlewareType is not null)
                {
                    var typedMiddleware = (IMiddleware<TParameter>)ActivatorUtilities.CreateInstance(_serviceProvider, type.NextMiddlewareType);

                    return typedMiddleware.InvokeAsync(parameter, action);
                }

                if (type.NextFunc is not null)
                {
                    return type.NextFunc(_serviceProvider, _ => action())(parameter);
                }

                throw new InvalidOperationException("Invalid pipeline component. No middlware or delegate supplied.");
            };

            return action();
        }
    }
}