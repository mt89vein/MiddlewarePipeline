using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Middlewares
{
    /// <summary>
    /// Extension methods for <see cref="IServiceCollection"/>.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Configures pipeline for <typeparamref name="TParameter"/>.
        /// </summary>
        /// <typeparam name="TParameter">Pipeline parameter type.</typeparam>
        /// <param name="services">Services configurator.</param>
        /// <param name="serviceLifetime">Pipeline lifetime scope.</param>
        /// <returns>A reference to the builder after the operation has completed.</returns>
        public static IPipelineBuilder<TParameter> ConfigurePipelineFor<TParameter>(
            this IServiceCollection services,
            ServiceLifetime serviceLifetime = ServiceLifetime.Scoped
        ) where TParameter : notnull
        {
            var builder = new PipelineBuilder<TParameter>();

            services.AddSingleton<IPipelineInfoAccessor<TParameter>>(builder);
            services.TryAdd(
                new ServiceDescriptor(typeof(IPipeline<TParameter>),
                    typeof(Pipeline<TParameter>),
                    serviceLifetime)
            );

            return builder;
        }
    }
}
