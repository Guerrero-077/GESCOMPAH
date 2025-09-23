using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace WebGESCOMPAH.Middleware.Handlers
{
    public class UnauthorizedAccessHandler : IExceptionHandler
    {
        public int Priority => 40;

        public bool CanHandle(Exception exception)
            => exception is UnauthorizedAccessException;

        public (ProblemDetails Problem, int StatusCode) Handle(Exception exception, IHostEnvironment env, HttpContext http)
        {
            var statusCode = (int)HttpStatusCode.Unauthorized;

            var detail = string.IsNullOrWhiteSpace(exception.Message)
                ? "No tienes permisos para acceder a este recurso."
                : exception.Message;

            var problem = ProblemDetailsFactory.Create(
                statusCode,
                title: "Acceso no autorizado.",
                detail: detail,
                type: "https://tools.ietf.org/html/rfc7235#section-3.1",
                instance: http.Request.Path
            );

            return (problem, statusCode);
        }
    }
}
