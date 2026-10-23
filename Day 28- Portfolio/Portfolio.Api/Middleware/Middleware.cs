using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Portfolio.Api.Middleware;

// Logs every request: method, path, status code and how long it took.
// (Same idea as the logging layer in the Node.js middleware of Project 1.)
public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var watch = Stopwatch.StartNew();
        await _next(context);
        watch.Stop();

        if (context.Request.Path.StartsWithSegments("/api"))
        {
            _logger.LogInformation("{Method} {Path}{Query} -> {Status} in {Ms} ms",
                context.Request.Method, context.Request.Path, context.Request.QueryString,
                context.Response.StatusCode, watch.ElapsedMilliseconds);
        }
    }
}

// Catches any unhandled exception and returns a clean 500 ProblemDetails JSON
// instead of a stack trace.
public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception for {Path}", context.Request.Path);

            if (context.Response.HasStarted) throw;

            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            await context.Response.WriteAsJsonAsync(new ProblemDetails
            {
                Status = 500,
                Title = "Something went wrong on the server.",
                Instance = context.Request.Path
            });
        }
    }
}

public static class MiddlewareExtensions
{
    public static IApplicationBuilder UseRequestLogging(this IApplicationBuilder app) =>
        app.UseMiddleware<RequestLoggingMiddleware>();

    public static IApplicationBuilder UseGlobalExceptionHandling(this IApplicationBuilder app) =>
        app.UseMiddleware<ExceptionHandlingMiddleware>();
}
