using Microsoft.AspNetCore.Mvc;
using System.Net;
using Utilities.Exceptions;

namespace WebGESCOMPAH.Middleware.Handlers
{
    public class ExternalServiceExceptionHandler : IExceptionHandler
    {
        public bool CanHandle(Exception exception) =>
            exception is ExternalServiceException;

        public (ProblemDetails Problem, int StatusCode) Handle(Exception exception, IHostEnvironment env)
        {
            var externalError = (ExternalServiceException)exception;

            var statusCode = (int)HttpStatusCode.ServiceUnavailable;

            var problem = new ProblemDetails
            {
                Status = statusCode,
                Title = "Error en servicio externo.",
                Detail = externalError.Message,
                Type = "https://tools.ietf.org/html/rfc7231#section-6.6.4"
            };

            return (problem, statusCode);
        }
    }
}