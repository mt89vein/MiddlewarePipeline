using Domain.Enums;
using Domain.Models;
using Domain.Subdomain1.Middlewares;
using Middlewares;
using System;

namespace Domain.Subdomain1
{
    public static class ServiceCollectionExtensions
    {
        private static IPipeline<SomeContext> _subdomain1Pipeline;

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

        /// <summary>
        /// Creates pipeline from current components.
        /// </summary>
        /// <param name="sp">Application service provider.</param>
        private static IPipeline<SomeContext> BuildPipeline(IServiceProvider sp)
        {
            if (_subdomain1Pipeline is not null)
            {
                return _subdomain1Pipeline;
            }

            var pipeline = new PipelineBuilder<SomeContext>();

            pipeline.Use<Subdomain1Middleware>();

            // chain other subdomain 1 middlewares

            return _subdomain1Pipeline = pipeline.Build(sp);
        }
    }
}
