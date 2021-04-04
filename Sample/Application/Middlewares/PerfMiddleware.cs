using Domain.Models;
using Microsoft.Extensions.Logging;
using Middlewares;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Middlewares
{
    public class PerfMiddleware : IMiddleware<SomeContext>
    {
        private readonly ILogger<PerfMiddleware> _logger;

        public PerfMiddleware(ILogger<PerfMiddleware> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Invokes middleware.
        /// </summary>
        /// <param name="parameter">Pipeline parameter.</param>
        /// <param name="next">Next middleware.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        public async Task InvokeAsync(SomeContext parameter, NextMiddleware next, CancellationToken cancellationToken)
        {
            var sw = new Stopwatch();
            sw.Start();

            try
            {
                await next();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Exception!");
            }
            finally
            {
                sw.Stop();
                _logger.LogInformation("Pipeline time {Time}", sw.ElapsedMilliseconds);
            }
        }
    }
}