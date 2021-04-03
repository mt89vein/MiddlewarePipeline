using Domain.Models;
using Microsoft.Extensions.Logging;
using Middlewares;
using System.Threading.Tasks;

namespace Application.Middlewares
{
    public class PreconditionCheckMiddleware : IMiddleware<SomeContext>
    {
        private readonly ILogger<PreconditionCheckMiddleware> _logger;

        public PreconditionCheckMiddleware(ILogger<PreconditionCheckMiddleware> logger)
        {
            _logger = logger;
        }

        public async Task InvokeAsync(SomeContext parameter, NextMiddleware next)
        {
            _logger.LogInformation("before PreconditionCheckMiddleware next");

            if (parameter.Request is null)
            {
                // also can supply additional error message ...

                parameter.Response.IsValid = false;

                return;
            }

            await next();

            _logger.LogInformation("after PreconditionCheckMiddleware next");
        }
    }
}