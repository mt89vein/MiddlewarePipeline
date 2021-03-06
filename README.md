MiddlewarePipeline
=============

[![NuGet version (MiddlewarePipeline)](https://img.shields.io/nuget/v/MiddlewarePipeline.svg?style=flat-square)](https://www.nuget.org/packages/MiddlewarePipeline)
![UnitTest](https://github.com/mt89vein/MiddlewarePipeline/workflows/UnitTest/badge.svg)
[![Coverage Status](https://coveralls.io/repos/github/mt89vein/MiddlewarePipeline/badge.svg?branch=master&v=1)](https://coveralls.io/github/mt89vein/MiddlewarePipeline?branch=master)

Set of classes and interfaces for configuring and executing custom pipeline using middleware pattern.

If you familiar with ASP.NET Core HttpContext pipeline and middlewares, then you will notice that this library is the similar thing, but aimed at building your custom pipelines using power of middlewares.

[![ASP.NET Core middleware pipeline execution order](https://docs.microsoft.com/en-US/aspnet/core/fundamentals/middleware/index/_static/request-delegate-pipeline.png)](https://docs.microsoft.com/en-US/aspnet/core/fundamentals/middleware/)

Look at unit tests for additional usage examples.

### Installing MiddlewarePipeline

You should install [MiddlewarePipeline with NuGet](https://www.nuget.org/packages/MiddlewarePipeline):

    Install-Package MiddlewarePipeline -Version 2.0.0

via the .NET Core command line interface:

    dotnet add package MiddlewarePipeline --version 2.0.0

or package reference

    <PackageReference Include="MiddlewarePipeline" Version="2.0.0" />

## Use cases

Imagine that you have complex request processing and its execution flow sometimes may differ from subdomain to subdomain.

e.g. you have common logic for some parts of request processing, which you can easily cover by abstract factory pattern or with strategy. Then, in other request processing part you detect that this request should be processed by second subdomain in a some unique way, and finally returns to common logic part.

let's assume we have this:

```csharp
public interface IDomain
{
    void DoOneThing(Param param);

    void DoSomethingElse(Param param);

    // and some more methods
}

public class Param {}
```

and have some separated subdomains, that should implement this interface

```csharp
public class FirstSubdomain : IDomain { /* here methods */ }

public class SecondSubdomain : IDomain { /* here methods */ }
```

but second subdomain have some additional business logic between DoOneThing and DoSomethingElse.. which is not fits to any of this methods. And you may want to extend IDomain interface with "BeforeXXX" "AfterXXX" methods. Ohhh... use pipelines and middlewares instead!)

With MiddlewarePipeline you can do this:

```csharp
// configuring execution pipeline order:
services.ConfigurePipelineFor<Param>()
        /*.Use<PerformanceMetricMiddleware>() */
        .Use<CommonMiddleware>() // common logic part
        .Use<SubdomainDetectMiddleware>() // should define, which subdomain must handle request
        .UseWhen<Param, FirstMiddleware>(p => p.Type = SubDomainType.First) // when predicate returns true, then FirstMiddleware will be executed
        .UseWhen<Param, SecondMiddleware>(p => p.Type = SubDomainType.Second)
        .Use<CommonFinalizeMiddleware>(); // finalization of pipeline

// as you can see, it is very easy to add custom logic in any part of request execution

// execution pipeline looks like this:
// 1. Before next() delegate in CommonMiddleware
// 2. Before next() delegate in SubdomainDetectMiddleware
// 3. If Type == SubDomainType.First, then executes FirstMiddleware, or skip otherwise and implicit execute next()
// 4. If Type == SubDomainType.Second, then executes SecondMiddleware, or skip otherwise and implicit execute next()
// 5. Before next() delegate in CommonFinalyzeMiddleware
// 6. After next() delegate in CommonFinalyzeMiddleware
// 7. After next() delegate in SecondMiddleware if it in pipeline
// 8. After next() delegate in FirstMiddleware if it in pipeline
// 9. After next() delegate in SubdomainDetectMiddleware
// 10. After next() delegate in CommonMiddleware

// middleware must implement IMiddleware<T> interface. Dependency injection via constructor is also supported.
public class FirstMiddleware : IMiddleware<Param>
{
    public async Task InvokeAsync(Param param, NextMiddleware next, CancellationToken cancellationToken)
    {
        // do something useful (before next middlewares)

        // invoke next middleware in pipeline
        await next();

        // do something useful (after next middlewares)
    }
}

// how to execute pipeline?

[Route("api/[controller]")]
public class ExampleController : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Get(Param param, [FromServices] IPipeline<Param> pipeline)
    {
        await pipeline.ExecuteAsync(param);

        return Ok(param);
    }
}
```

If you need pipeline branching, you can extend it with building your own pipeline using PipelineBuilder class, create Pipeline class from it, and execute.

look at [Manually build pipeline section](#manually-build-pipeline) or [ServiceCollectionExtensions.cs](https://github.com/mt89vein/MiddlewarePipeline/blob/master/Sample/Application/ServiceCollectionExtensions.cs) for more advanced examples

## Features

### Lambda as middleware

```csharp
services.ConfigurePipelineFor<Param>()
    /* other middlewares */
    .Use(async (param, next, cancellationToken) =>
    {
        // before

        await next();

        // after
    });
    /* other middlewares */
```

### Lambda as middleware with IServiceProvider

```csharp
services.ConfigurePipelineFor<Param>()
    /* other middlewares */
    .Use((sp, next) =>
    {
        var dep = sp.GetRequiredService<Dependency>();

        return async (param, cancellationToken) =>
        {
            // before

            await dep.DoSomethingAsync(param, cancellationToken);

            await next(param, cancellationToken);

            // after
        };
    });
    /* other middlewares */
```

### Lambda as middleware with injecting via param

```csharp
services.AddTransient<Dependency>();
services.ConfigurePipelineFor<Param>()
    /* other middlewares */
    // inject dependencies from DI (up to 3)
    .Use<Param, Dependency>(async (param, dependency, next, cancellationToken) =>
    {
        // before

        await next();

        // after
    });
    /* other middlewares */
```

### Conditional middleware execution

CondMiddleware is only executed if its predicate returns true.
In this example, if `ShouldExecute()` returns true, otherwise the CondMiddleware is skipped and implicitly performs next() to continue the pipeline.

```csharp

services.ConfigurePipelineFor<Param>()
    /* other middlewares */
    .UseWhen<Param, CondMiddleware>(p => p.ShouldExecute());
    /* other middlewares */
 ```

Same as above, but here lambda as conditional middleware

 ```csharp
services.ConfigurePipelineFor<Param>()
    /* other middlewares */
    .UseWhen(p => p.ShouldExecute(), async (ctx, next, cancellationToken) =>
    {
        // before

        await next();

        // after
    });
    /* other middlewares */
```

### Manually build pipeline

if you want to build of pipeline manually (e.g. for conditional execution)
you can use this snippet:

```csharp
/// <summary>
/// Register first subdomain pipeline middlewares.
/// </summary>
/// <param name="pipelineBuilder">Pipeline builder.</param>
/// <returns>A reference to the builder after the operation has completed.</returns>
public static IPipelineBuilder<Param> UseFirstSubdomainPipeline(
    this IPipelineBuilder<Param> pipelineBuilder
)
{
    return pipelineBuilder.Use((sp, next) =>
    {
        return async (param, cancellationToken) =>
        {
            // like conditional middleware, execute additional pipeline branch
            if (param.Type == SubDomainType.First)
            {
                await BuildPipeline(sp).ExecuteAsync(param, cancellationToken);
            }

            // then execute other parts
            await next(param, cancellationToken);
        };
    });
}

/// <summary>
/// Creates pipeline from current components.
/// </summary>
/// <param name="sp">Application service provider.</param>
/// <returns>Configured pipeline.</returns>
private static IPipeline<Param> BuildPipeline(IServiceProvider sp)
{
    var pipelineBuilder = new PipelineBuilder<Param>();

    pipelineBuilder
        .Use<Middleware1>()
        .Use<Middleware2>();
        /* chain other middlewares */

    return pipelineBuilder.Build(sp);
}

```
and register in common pipeline:

```diff 
services.ConfigurePipelineFor<Param>()
    /* other middlewares */
    .Use<PerfMiddleware>()
    .Use<PreconditionCheckMiddleware>()
    .Use<SubdomainDetectMiddleware>()
+   .UseFirstSubdomainPipeline()
    .UseSecondSubdomainPipeline();
    /* other middlewares */;
```

### Using pipeline without IServiceProvider:

```csharp
var pipelineBuilder = new PipelineBuilder<TestCtx>();

pipelineBuilder
    .Use(_ => new Middleware1())
    .Use(async (ctx, next, cancellationToken) => {

        // do something
        await next();
        // do more
    });

pipeline = pipelineBuilder.Build();
// build method checks for ServiceProvider requirement in any of registered middlewares.

await pipeline.ExecuteAsync(new TestCtx(), CancellationToken.None);
```

### Contribute

Feel free for creation issues, or PR :)
