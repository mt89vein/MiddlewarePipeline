using System.Threading;
using System.Threading.Tasks;

namespace Middlewares.Tests.TestMiddlewares
{
    public class Middleware2 : IMiddleware<TestCtx>
    {
        public async Task InvokeAsync(TestCtx parameter, NextMiddleware next, CancellationToken cancellationToken)
        {
            parameter.ExecutedMiddlewaresCount++;

            parameter.Msg+= "Before_" + nameof(Middleware2);

            await next();

            parameter.Msg+= "After_" + nameof(Middleware2);
        }
    }
}