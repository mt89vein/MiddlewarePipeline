using Domain.Models;
using Microsoft.Extensions.Logging;
using Middlewares;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Middlewares
{
    public class UnhandledExceptionMiddleware<TParameter> : IMiddleware<TParameter>
    {
        private readonly ILogger<UnhandledExceptionMiddleware<TParameter>> _logger;

        public UnhandledExceptionMiddleware(ILogger<UnhandledExceptionMiddleware<TParameter>> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Invokes middleware.
        /// </summary>
        /// <param name="parameter">Pipeline parameter.</param>
        /// <param name="next">Next middleware.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        public async Task InvokeAsync(TParameter parameter, NextMiddleware next, CancellationToken cancellationToken)
        {
            try
            {
                await next();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Unhandled exception!");
            }
        }
    }
}