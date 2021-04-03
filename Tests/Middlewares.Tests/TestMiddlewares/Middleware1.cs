using System.Threading.Tasks;

namespace Middlewares.Tests.TestMiddlewares
{
    public class Middleware1 : IMiddleware<TestCtx>
    {
        public async Task InvokeAsync(TestCtx parameter, NextMiddleware next)
        {
            parameter.ExecutedMiddlewaresCount++;

            parameter.Msg+= "Before_" + nameof(Middleware1);

            await next();

            parameter.Msg+= "After_" + nameof(Middleware1);
        }
    }
}