using Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Middlewares;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestController : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> Get([FromServices] IPipeline<SomeContext> pipeline, CancellationToken cancellationToken)
        {
            var ctx = new SomeContext(new SomeRequest
            {
                Documents = new List<Document>
                {
                    new()
                    {
                        FileId = new Guid(),
                        FileName = "document_for_user.xml"
                    }
                }
            });

            await pipeline.ExecuteAsync(ctx, cancellationToken);

            return Ok(ctx);
        }
    }
}
