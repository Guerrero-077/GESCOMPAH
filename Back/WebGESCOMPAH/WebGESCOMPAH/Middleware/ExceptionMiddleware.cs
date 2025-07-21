using FluentValidation;
using System.Net;
using System.Text.Json;
using Utilities.Exceptions;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
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
            _logger.LogError(ex, "Excepción no manejada");

            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var response = context.Response;
        object result;

        switch (exception)
        {
            case FluentValidation.ValidationException vex:
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                result = new
                {
                    isSuccess = false,
                    statusCode = 400,
                    message = "Errores de validación.",
                    errors = vex.Errors.Select(e => new
                    {
                        field = e.PropertyName,
                        error = e.ErrorMessage
                    })
                };
                break;

            case BusinessException bex:
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                result = new
                {
                    isSuccess = false,
                    statusCode = 400,
                    message = bex.Message
                };
                break;

            case UnauthorizedAccessException:
                response.StatusCode = (int)HttpStatusCode.Unauthorized;
                result = new
                {
                    isSuccess = false,
                    statusCode = 401,
                    message = "Acceso no autorizado."
                };
                break;

            default:
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                result = new
                {
                    isSuccess = false,
                    statusCode = 500,
                    message = "Error interno del servidor.",
                    detail = exception.Message
                };
                break;
        }

        var json = JsonSerializer.Serialize(result, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        });

        await context.Response.WriteAsync(json);
    }
}
