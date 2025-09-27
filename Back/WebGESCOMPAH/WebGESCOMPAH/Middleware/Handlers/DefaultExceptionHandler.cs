using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace WebGESCOMPAH.Middleware.Handlers
{
    public class DefaultExceptionHandler : IExceptionHandler
    {
        public int Priority => int.MaxValue;

        public bool CanHandle(Exception exception) => true;

        public (ProblemDetails Problem, int StatusCode) Handle(Exception exception, IHostEnvironment env, HttpContext http)
        {
            var statusCode = (int)HttpStatusCode.InternalServerError;

            var detail = "Ocurrió un error inesperado.";

            var problem = ProblemDetailsFactory.Create(
                statusCode,
                title: "Error interno del servidor.",
                detail: detail,
                type: "https://tools.ietf.org/html/rfc7231#section-6.6.1",
                instance: http.Request.Path
            );

            return (problem, statusCode);
        }
    }
}
