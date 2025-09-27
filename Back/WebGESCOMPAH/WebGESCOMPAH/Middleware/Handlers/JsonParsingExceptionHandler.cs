using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.Json;

namespace WebGESCOMPAH.Middleware.Handlers
{
    public class JsonParsingExceptionHandler : IExceptionHandler
    {
        public int Priority => 12;

        public bool CanHandle(Exception exception) => exception is JsonException;

        public (ProblemDetails Problem, int StatusCode) Handle(Exception exception, IHostEnvironment env, HttpContext http)
        {
            var statusCode = (int)HttpStatusCode.BadRequest;

            var detail = "El cuerpo de la solicitud no es un JSON válido.";

            var problem = ProblemDetailsFactory.Create(
                statusCode,
                title: "Error de parseo JSON.",
                detail: detail,
                type: "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                instance: http.Request.Path
            );

            return (problem, statusCode);
        }
    }
}
