using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace WebGESCOMPAH.Middleware.Handlers
{
    public class HttpRequestExceptionHandler : IExceptionHandler
    {
        public int Priority => 47;

        public bool CanHandle(Exception exception)
            => exception is HttpRequestException;

        public (ProblemDetails Problem, int StatusCode) Handle(Exception exception, IHostEnvironment env, HttpContext http)
        {
            var statusCode = (int)HttpStatusCode.BadGateway;

            var detail = "Fallo la comunicación con un servicio externo.";

            var problem = ProblemDetailsFactory.Create(
                statusCode,
                title: "Error al comunicarse con servicio externo.",
                detail: detail,
                type: "https://www.rfc-editor.org/rfc/rfc7231#section-6.6.3",
                instance: http.Request.Path
            );

            return (problem, statusCode);
        }
    }
}
