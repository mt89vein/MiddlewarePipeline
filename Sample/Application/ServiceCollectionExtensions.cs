using Application.Middlewares;
using Domain.Models;
using Domain.Subdomain1;
using Microsoft.Extensions.DependencyInjection;
using Middlewares;

namespace Application
{
    public static class ServiceCollectionExtensions
    {
        public static void AddPipeline(this IServiceCollection services)
        {
            services.ConfigurePipelineFor<SomeContext>()
                .Use<PerfMiddleware>()
                .Use<PreconditionCheckMiddleware>()
                .Use<SubdomainDetectMiddleware>()
                .UseSubdomain1Pipeline()
                // .UseSubdomain2Pipeline()
                .Use(async (_, next) =>
                {
                    // this is last -> finalyzer

                    // before

                    await next();

                    // after
                });
        }
    }
}