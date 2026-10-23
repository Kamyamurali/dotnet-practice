using System.Text.Json;
using BankingApi.DTOs;
using BankingApi.Exceptions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace BankingApi.Middleware;

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
            await HandleAsync(context, ex);
        }
    }

    private async Task HandleAsync(HttpContext context, Exception ex)
    {
        (int status, string message) = ex switch
        {
            AppException app => (app.StatusCode, app.Message),

            DbUpdateException { InnerException: SqlException sql } when sql.Number is 2627 or 2601
                => (StatusCodes.Status409Conflict, "This record already exists."),

            DbUpdateException { InnerException: SqlException sql } when sql.Number == 547
                => (StatusCodes.Status400BadRequest, "The data breaks a database rule."),

            SqlException or DbUpdateException
                => (StatusCodes.Status503ServiceUnavailable, "The database is unavailable. Please try again shortly."),

            _ => (StatusCodes.Status500InternalServerError, "Something went wrong on the server.")
        };

        if (status >= 500)
            _logger.LogError(ex, "Unhandled error on {Method} {Path}", context.Request.Method, context.Request.Path);
        else
            _logger.LogWarning("Request failed with {Status}: {Message}", status, message);

        context.Response.StatusCode = status;
        context.Response.ContentType = "application/json";

        var body = new ErrorResponse { StatusCode = status, Message = message };
        await context.Response.WriteAsync(JsonSerializer.Serialize(body,
            new JsonSerializerOptions(JsonSerializerDefaults.Web)));
    }
}
