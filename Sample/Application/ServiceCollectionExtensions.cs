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
            var builder = new PipelineBuilder<SomeContext>();
            builder.Use(new SubdomainDetectMiddleware(null));
            builder.Use((_, _) => new SubdomainDetectMiddleware(null));

            builder.Build();

            services.ConfigurePipelineFor<SomeContext>()
                .Use<UnhandledExceptionMiddleware<SomeContext>>()
                .Use<PerfMiddleware<SomeContext>>()
                .Use<PreconditionCheckMiddleware>()
                .Use<SubdomainDetectMiddleware>()
                .Use((_, _) => new SubdomainDetectMiddleware(null))
                .Use(new SubdomainDetectMiddleware(null))
                .UseSubdomain1Pipeline()
                // .UseSubdomain2Pipeline()
                .Use(async (_, next, ct) =>
                {
                    // this is last -> finalizer

                    // before

                    await next();

                    // after
                });
        }
    }
}