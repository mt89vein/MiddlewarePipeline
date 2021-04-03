using Middlewares;
using System.Threading.Tasks;

namespace UnitTests.TestMiddlewares
{
    public class Middleware2 : IMiddleware<TestCtx>
    {
        public async Task InvokeAsync(TestCtx parameter, NextMiddleware next)
        {
            parameter.ExecutedMiddlewaresCount++;

            parameter.Msg+= "Before_" + nameof(Middleware2);

            await next();

            parameter.Msg+= "After_" + nameof(Middleware2);
        }
    }
}