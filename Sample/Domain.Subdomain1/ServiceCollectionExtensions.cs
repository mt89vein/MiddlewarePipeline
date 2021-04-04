using Domain.Enums;
using Domain.Models;
using Domain.Subdomain1.Middlewares;
using Middlewares;
using System;

namespace Domain.Subdomain1
{
    public static class ServiceCollectionExtensions
    {
        public static IPipelineBuilder<SomeContext> UseSubdomain1Pipeline(
            this IPipelineBuilder<SomeContext> pipelineBuilder
        )
        {
            return pipelineBuilder.Use((sp, next) =>
            {
                return async (ctx, cancellationToken) =>
                {
                    if (ctx.Response.Type == SubDomainType.Subdomain1)
                    {
                        await BuildPipeline(sp).ExecuteAsync(ctx, cancellationToken);
                    }

                    await next(ctx, cancellationToken);
                };
            });
        }

        private static IPipeline<SomeContext> BuildPipeline(IServiceProvider sp)
        {
            var pipeline = new PipelineBuilder<SomeContext>();

            pipeline.Use<Subdomain1Middleware>();

            // chain other subdomain 1 middlewares

            return new Pipeline<SomeContext>(sp, pipeline);
        }
    }
}
