using Domain.Enums;
using Domain.Models;
using Microsoft.Extensions.Logging;
using Middlewares;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Middlewares
{
    public class SubdomainDetectMiddleware : IMiddleware<SomeContext>
    {
        private readonly ILogger<SubdomainDetectMiddleware> _logger;

        public SubdomainDetectMiddleware(ILogger<SubdomainDetectMiddleware> logger)
        {
            _logger = logger;
        }

        public async Task InvokeAsync(SomeContext parameter, NextMiddleware next, CancellationToken cancellationToken)
        {
            _logger.LogInformation("before SubdomainDetectMiddleware next");

            // imagine complex logic for detecting which subdomain should process this

            parameter.Response.Type = SubDomainType.Subdomain1;

            await next();

            _logger.LogInformation("after SubdomainDetectMiddleware next");
        }
    }
}