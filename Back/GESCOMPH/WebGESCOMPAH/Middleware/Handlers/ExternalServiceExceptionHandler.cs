using Microsoft.AspNetCore.Mvc;
using System.Net;
using Utilities.Exceptions;
using WebGESCOMPH.Middleware;

namespace WebGESCOMPH.Middleware.Handlers
{
    public class ExternalServiceExceptionHandler : IExceptionHandler
    {
        public int Priority => 50;
        public bool CanHandle(Exception exception) => exception is ExternalServiceException;

        public (ProblemDetails Problem, int StatusCode) Handle(Exception exception, IHostEnvironment env, HttpContext http)
        {
            var statusCode = (int)HttpStatusCode.ServiceUnavailable;
            var problem = new ProblemDetails
            {
                Status = statusCode,
                Title = "Error en servicio externo.",
                Detail = exception.Message,
                Type = "https://tools.ietf.org/html/rfc7231#section-6.6.4",
                Instance = http.Request.Path
            };
            return (problem, statusCode);
        }
    }
}
