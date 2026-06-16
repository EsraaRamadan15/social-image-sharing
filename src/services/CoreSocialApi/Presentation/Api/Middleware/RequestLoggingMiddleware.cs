using System.Diagnostics;
using System.Security.Claims;

namespace Api.Middleware;

public sealed class RequestLoggingMiddleware
{
    private const string CorrelationIdHeaderName = "X-Correlation-Id";

    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var correlationId = GetOrCreateCorrelationId(context);
        context.Response.Headers[CorrelationIdHeaderName] = correlationId;

        using var scope = _logger.BeginScope(new Dictionary<string, object?>
        {
            ["CorrelationId"] = correlationId,
            ["TraceId"] = Activity.Current?.Id ?? context.TraceIdentifier
        });

        var stopwatch = Stopwatch.StartNew();

        await _next(context);

        stopwatch.Stop();

        var statusCode = context.Response.StatusCode;
        var logLevel = statusCode >= StatusCodes.Status500InternalServerError
            ? LogLevel.Error
            : statusCode >= StatusCodes.Status400BadRequest
                ? LogLevel.Warning
                : LogLevel.Information;

        var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);

        _logger.Log(
            logLevel,
            "HTTP {Method} {Path} responded {StatusCode} in {ElapsedMilliseconds} ms for UserId {UserId}",
            context.Request.Method,
            context.Request.Path,
            statusCode,
            stopwatch.ElapsedMilliseconds,
            userId ?? "anonymous");
    }

    private static string GetOrCreateCorrelationId(HttpContext context)
    {
        if (context.Request.Headers.TryGetValue(CorrelationIdHeaderName, out var correlationId)
            && !string.IsNullOrWhiteSpace(correlationId))
        {
            return correlationId.ToString();
        }

        return context.TraceIdentifier;
    }
}
