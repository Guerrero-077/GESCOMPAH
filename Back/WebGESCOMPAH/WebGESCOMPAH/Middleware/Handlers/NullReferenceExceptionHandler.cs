using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace WebGESCOMPAH.Middleware.Handlers
{
    public class NullReferenceExceptionHandler : IExceptionHandler
    {
        // Ejecutar antes de Infrastructure/Default
        public int Priority => 90;

        public bool CanHandle(Exception exception)
            => exception is NullReferenceException;

        public (ProblemDetails Problem, int StatusCode) Handle(Exception exception, IHostEnvironment env, HttpContext http)
        {
            var statusCode = (int)HttpStatusCode.InternalServerError;

            // Mensaje claro y corto, sin stacktrace
            var detail = "Se intentó acceder a un dato no disponible (referencia nula). Verifique que los datos relacionados estén completos.";

            var problem = ProblemDetailsFactory.Create(
                statusCode,
                title: "Error de referencia nula.",
                detail: detail,
                type: "https://tools.ietf.org/html/rfc7231#section-6.6.1",
                instance: http.Request.Path
            );

            return (problem, statusCode);
        }
    }
}

