using Microsoft.AspNetCore.Mvc;
using System.Net;
using Utilities.Exceptions;

namespace WebGESCOMPAH.Middleware.Handlers
{
    public class EntityNotFoundExceptionHandler : IExceptionHandler
    {
        public bool CanHandle(Exception exception) =>
            exception is EntityNotFoundException;

        public (ProblemDetails Problem, int StatusCode) Handle(Exception exception, IHostEnvironment env)
        {
            var notFoundException = (EntityNotFoundException)exception;

            var statusCode = (int)HttpStatusCode.NotFound;

            var problem = new ProblemDetails
            {
                Status = statusCode,
                Title = "Recurso no encontrado.",
                Detail = notFoundException.Message,
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.4"
            };

            return (problem, statusCode);
        }
    }
}