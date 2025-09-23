using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace WebGESCOMPAH.Middleware.Handlers
{
    public class InfrastructureExceptionHandler : IExceptionHandler
    {
        public int Priority => 95;

        public bool CanHandle(Exception exception)
            => exception is InvalidOperationException || exception is NotSupportedException;

        public (ProblemDetails Problem, int StatusCode) Handle(Exception exception, IHostEnvironment env, HttpContext http)
        {
            var statusCode = (int)HttpStatusCode.InternalServerError;

            var detail = env.IsDevelopment()
                ? exception.ToString()
                : "Ocurrió un error inesperado en la infraestructura del sistema.";

            var problem = ProblemDetailsFactory.Create(
                statusCode,
                title: "Error interno del sistema.",
                detail: detail,
                type: "https://tools.ietf.org/html/rfc7231#section-6.6.1",
                instance: http.Request.Path
            );

            return (problem, statusCode);
        }
    }
}
