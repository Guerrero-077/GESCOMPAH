using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace WebGESCOMPAH.Middleware.Handlers
{
    public class FileAccessExceptionHandler : IExceptionHandler
    {
        public int Priority => 80;

        public bool CanHandle(Exception exception)
            => exception is IOException || exception is FileNotFoundException;

        public (ProblemDetails Problem, int StatusCode) Handle(Exception exception, IHostEnvironment env, HttpContext http)
        {
            var statusCode = (int)HttpStatusCode.InternalServerError;

            var detail = "Ocurrió un error al acceder a un archivo o recurso físico.";

            var problem = ProblemDetailsFactory.Create(
                statusCode,
                title: "Error de acceso a archivo.",
                detail: detail,
                type: "https://tools.ietf.org/html/rfc7231#section-6.6.1",
                instance: http.Request.Path
            );

            return (problem, statusCode);
        }
    }
}
