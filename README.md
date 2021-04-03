MiddlewarePipeline
=============

Set of classes and interfaces for configuring and executing custom pipeline using middleware pattern.

If you familiar with ASP.NET Core HttpContext pipeline and middlewares, then you will notice that this library is the similar thing, but aimed at building your custom pipelines using power of middlewares.

[![NuGet version (MiddlewarePipeline)](https://img.shields.io/nuget/v/MiddlewarePipeline.svg?style=flat-square)](https://www.nuget.org/packages/MiddlewarePipeline)
![UnitTest](https://github.com/mt89vein/MiddlewarePipeline/workflows/UnitTest/badge.svg)
[![Coverage Status](https://coveralls.io/repos/github/mt89vein/MiddlewarePipeline/badge.svg?branch=master)](https://coveralls.io/github/mt89vein/MiddlewarePipeline?branch=master)

Look at tests for additional examples.

### Installing MiddlewarePipeline

You should install [MiddlewarePipeline with NuGet](https://www.nuget.org/packages/MiddlewarePipeline):

    Install-Package MiddlewarePipeline -Version 1.0.0

via the .NET Core command line interface:

    dotnet add package MiddlewarePipeline --version 1.0.0

or package reference

    <PackageReference Include="MiddlewarePipeline" Version="1.0.0" />

## Use cases

Imagine that you have complex request processing and its execution flow sometimes may differ from subdomain to subdomain.

e.g. you have common logic for some parts of request processing, which you can easily cover by abstract factory pattern or with strategy. Then, in other request processing part you detect that this request should be processed by second subdomain in a some unique way, and finally returns to common logic part.

let's assume we have this:

```csharp
public interface IDomain
{
    void DoOneThing(Param param);

    void DoSometingElse(Param param);

    // and some more methods
}
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
        .Use<CommonFinalyzeMiddleware>(); // finalization of pipeline

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

public class FirstMiddleware : IMiddleware<Param>
{
    public async Task InvokeAsync(Param param, NextMiddleware next)
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

look at [ServiceCollectionExtensions.cs](https://github.com/mt89vein/MiddlewarePipeline/blob/master/Sample/Application/ServiceCollectionExtensions.cs) for more advanced examples

## Features

### Lambda as middleware

```csharp
services.ConfigurePipelineFor<Param>()
    .Use(async (param, next) =>
    {
        // before

        await next();

        // after
    });
```

### Lambda as middleware with IServiceProvider

```csharp
services.ConfigurePipelineFor<Param>()
    .Use((sp, next) =>
    {
        var dep = sp.GetRequiredService<Dependency>();

        return async param =>
        {
            // before

            await dep.DoSomethingAsync(param);

            await next(param);

            // after
        };
    });
```

### Lambda as middleware with injecting via param

```csharp
services.AddTransient<Dependency>();
services.ConfigurePipelineFor<Param>()
    // inject dependencies from DI (up to 3)
    .Use<Param, Dependency>(async (param, dependency, next) =>
    {
        // before

        await next();

        // after
    });
```

### Contribute

Feel free for creation issues, or PR :)
