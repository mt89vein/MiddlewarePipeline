using System.Threading;
using System.Threading.Tasks;

namespace Middlewares.Tests.TestMiddlewares
{
    public class Middleware1 : IMiddleware<TestCtx>
    {
        public async Task InvokeAsync(TestCtx parameter, NextMiddleware next, CancellationToken cancellationToken)
        {
            parameter.ExecutedMiddlewaresCount++;

            parameter.Msg+= "Before_" + nameof(Middleware1);

            await next();

            parameter.Msg+= "After_" + nameof(Middleware1);
        }
    }
}