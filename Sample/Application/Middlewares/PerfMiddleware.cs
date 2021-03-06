using Domain.Models;
using Microsoft.Extensions.Logging;
using Middlewares;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Middlewares
{
    public class PerfMiddleware<TParameter> : IMiddleware<TParameter>
    {
        private readonly ILogger<PerfMiddleware<TParameter>> _logger;

        public PerfMiddleware(ILogger<PerfMiddleware<TParameter>> logger)
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
            var sw = new Stopwatch();
            sw.Start();

            try
            {
                await next();
            }
            finally
            {
                sw.Stop();
                _logger.LogInformation("Pipeline time {Time}", sw.ElapsedMilliseconds);

                // or _metricCollector.AddPipelineDurationMeasurement(typeof(TParameter).Name, sw.ElapsedMilliseconds);
            }
        }
    }
}