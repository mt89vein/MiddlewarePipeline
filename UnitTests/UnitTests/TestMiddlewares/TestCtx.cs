namespace UnitTests.TestMiddlewares
{
    public class TestCtx
    {
        public string Msg { get; set; }

        public int ExecutedMiddlewaresCount { get; set; }

        public bool ExecuteMiddleware2 { get; set; } = true;
    }
}