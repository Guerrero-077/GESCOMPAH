using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace WebGESCOMPAH.Middleware.Handlers
{
    public class UnauthorizedAccessHandler : IExceptionHandler
    {
        public bool CanHandle(Exception exception) =>
            exception is UnauthorizedAccessException;

        public (ProblemDetails Problem, int StatusCode) Handle(Exception exception, IHostEnvironment env)
        {
            var statusCode = (int)HttpStatusCode.Unauthorized;

            var problem = new ProblemDetails
            {
                Status = statusCode,
                Title = "Acceso no autorizado.",
                Detail = "No tienes permisos para acceder a este recurso.",
                Type = "https://tools.ietf.org/html/rfc7235#section-3.1"
            };

            return (problem, statusCode);
        }
    }
}