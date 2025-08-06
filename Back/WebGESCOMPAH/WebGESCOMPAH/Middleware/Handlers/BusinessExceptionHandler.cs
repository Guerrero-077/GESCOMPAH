using Microsoft.AspNetCore.Mvc;
using System.Net;
using Utilities.Exceptions;

namespace WebGESCOMPAH.Middleware.Handlers
{
    public class BusinessExceptionHandler : IExceptionHandler
    {
        public bool CanHandle(Exception exception) =>
            exception is BusinessException;

        public (ProblemDetails Problem, int StatusCode) Handle(Exception exception, IHostEnvironment env)
        {
            var businessException = (BusinessException)exception;

            var statusCode = (int)HttpStatusCode.BadRequest;

            var problem = new ProblemDetails
            {
                Status = statusCode,
                Title = "Error de negocio.",
                Detail = businessException.Message,
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1"
            };

            return (problem, statusCode);
        }
    }
}