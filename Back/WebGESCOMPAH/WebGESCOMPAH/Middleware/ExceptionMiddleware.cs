using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Text.Json;
using Utilities.Exceptions;
using ValidationException = FluentValidation.ValidationException;

namespace ExceptionHandling
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;
        private readonly IHostEnvironment _env;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, IHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _env = env;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Excepción no manejada en la ruta: {Path}", context.Request.Path);
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            var response = context.Response;
            object result;

            switch (exception)
            {
                case ValidationException vex:
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    result = new
                    {
                        isSuccess = false,
                        statusCode = 400,
                        message = "Errores de validación.",
                        errors = vex.Errors.Select(e => new { field = e.PropertyName, error = e.ErrorMessage })
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

                case NotFoundException nf:
                    response.StatusCode = (int)HttpStatusCode.NotFound;
                    result = new
                    {
                        isSuccess = false,
                        statusCode = 404,
                        message = nf.Message
                    };
                    break;


                default:
                    response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    result = new
                    {
                        isSuccess = false,
                        statusCode = 500,
                        message = "Error interno del servidor.",
                        detail = _env.IsDevelopment() ? exception.Message : null
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

    public static class ExceptionMiddlewareExtensions
    {
        public static IApplicationBuilder UseGlobalExceptionHandling(this IApplicationBuilder app)
        {
            return app.UseMiddleware<ExceptionMiddleware>();
        }
    }
}
