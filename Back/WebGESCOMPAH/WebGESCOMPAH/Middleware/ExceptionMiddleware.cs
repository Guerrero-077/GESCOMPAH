using System.Text.Json;
using WebGESCOMPAH.Middleware;
using WebGESCOMPAH.Middleware.Handlers;

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
            _logger.LogError(ex, "Excepción no manejada en la ruta: {Path}", context.Request.Path);
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/problem+json";

        var handler = _handlers.FirstOrDefault(h => h.CanHandle(exception))
                      ?? new DefaultExceptionHandler();

        var (problem, statusCode) = handler.Handle(exception, _env);
        context.Response.StatusCode = statusCode;

        var json = JsonSerializer.Serialize(problem, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        });

        await context.Response.WriteAsync(json);
    }
}
