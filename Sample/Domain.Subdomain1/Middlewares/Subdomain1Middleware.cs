using Domain.Models;
using Domain.Subdomain1.Models;
using Middlewares;
using System.Threading;
using System.Threading.Tasks;

namespace Domain.Subdomain1.Middlewares
{
    public class Subdomain1Middleware : IMiddleware<SomeContext>
    {
        /// <summary>
        /// Invokes middleware.
        /// </summary>
        /// <param name="parameter">Pipeline parameter.</param>
        /// <param name="next">Next middleware.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        public Task InvokeAsync(SomeContext parameter, NextMiddleware next, CancellationToken cancellationToken)
        {
            foreach (var doc in parameter.Request.Documents)
            {
                parameter.Response.Documents.Add(new Subdomain1Document
                {
                    FileName = doc.FileName,
                    FileId = doc.FileId,
                    AdditionalInfo = "some additional data"
                });
            }

            return next();
        }
    }
}
