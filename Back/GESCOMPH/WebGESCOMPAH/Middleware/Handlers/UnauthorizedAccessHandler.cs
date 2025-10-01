using Microsoft.AspNetCore.Mvc;
using System.Net;
using WebGESCOMPH.Middleware;

namespace WebGESCOMPH.Middleware.Handlers
{
    public class UnauthorizedAccessHandler : IExceptionHandler
    {
        public int Priority => 40;
        public bool CanHandle(Exception exception) => exception is UnauthorizedAccessException;

        public (ProblemDetails Problem, int StatusCode) Handle(Exception exception, IHostEnvironment env, HttpContext http)
        {
            var statusCode = (int)HttpStatusCode.Unauthorized;
            var problem = new ProblemDetails
            {
                Status = statusCode,
                Title = "Acceso no autorizado.",
                // Superficialmente genérico, pero detallamos la causa con el mensaje de la excepción
                Detail = string.IsNullOrWhiteSpace(exception.Message)
                    ? "No tienes permisos para acceder a este recurso."
                    : exception.Message,
                Type = "https://tools.ietf.org/html/rfc7235#section-3.1",
                Instance = http.Request.Path
            };
            return (problem, statusCode);
        }
    }
}
