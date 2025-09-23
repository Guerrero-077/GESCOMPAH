using Microsoft.AspNetCore.Mvc;
using System.Net;
using Utilities.Exceptions;

namespace WebGESCOMPAH.Middleware.Handlers
{
    public class ExternalServiceExceptionHandler : IExceptionHandler
    {
        public int Priority => 50;

        public bool CanHandle(Exception exception)
            => exception is ExternalServiceException;

        public (ProblemDetails Problem, int StatusCode) Handle(Exception exception, IHostEnvironment env, HttpContext http)
        {
            var statusCode = (int)HttpStatusCode.ServiceUnavailable;

            var problem = ProblemDetailsFactory.Create(
                statusCode,
                title: "Error en servicio externo.",
                detail: exception.Message,
                type: "https://tools.ietf.org/html/rfc7231#section-6.6.4",
                instance: http.Request.Path
            );

            return (problem, statusCode);
        }
    }
}
