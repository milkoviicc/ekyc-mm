using System.Net;
using eKYC.Domain.Exceptions;

namespace eKYC.Api.Middleware;

/// <summary>
/// Maps domain exceptions to HTTP status codes and ensures no stack trace ever reaches the client
/// (CLAUDE.md: "Never expose stack traces to API callers"). Unrecognized exceptions become a bare 500.
/// </summary>
public sealed class ExceptionHandlingMiddleware
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
        catch (ConcurrencyException ex)
        {
            _logger.LogWarning(ex, "Concurrency conflict");
            context.Response.StatusCode = (int)HttpStatusCode.Conflict;
            await context.Response.WriteAsJsonAsync(new
            {
                error = "concurrency_conflict",
                message = ex.Message,
                currentRecord = ex.CurrentRecordObject,
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception");
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            await context.Response.WriteAsJsonAsync(new { error = "internal_error", message = "An unexpected error occurred." });
        }
    }
}
