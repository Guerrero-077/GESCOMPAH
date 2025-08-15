using System.Text.Json;
using WebGESCOMPAH.Middleware;

public class ExceptionMiddleware : IMiddleware
{
    private readonly ILogger<ExceptionMiddleware> _logger;
    private readonly IHostEnvironment _env;
    private readonly IEnumerable<IExceptionHandler> _handlers;

    public ExceptionMiddleware(ILogger<ExceptionMiddleware> logger,
                               IHostEnvironment env,
                               IEnumerable<IExceptionHandler> handlers)
    {
        _logger = logger;
        _env = env;
        _handlers = handlers;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Excepción no manejada en {Path}", context.Request.Path);
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/problem+json";

        var handler = _handlers
            .OrderBy(h => h.Priority)
            .First(h => h.CanHandle(exception)); // siempre habrá al menos Default

        var (problem, statusCode) = handler.Handle(exception, _env, context);

        // metadatos útiles
        problem.Extensions ??= new Dictionary<string, object?>();
        problem.Extensions["traceId"] = context.TraceIdentifier;
        problem.Extensions["path"] = context.Request.Path.Value;
        problem.Extensions["method"] = context.Request.Method;
        if (context.Request.Headers.TryGetValue("X-Correlation-Id", out var corr))
            problem.Extensions["correlationId"] = corr.ToString();

        context.Response.StatusCode = statusCode;

        var json = JsonSerializer.Serialize(problem, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        });

        await context.Response.WriteAsync(json);
    }
}
