using Microsoft.AspNetCore.Mvc;
using System.Net;
using Utilities.Exceptions;

namespace WebGESCOMPAH.Middleware.Handlers
{
    public class EntityNotFoundExceptionHandler : IExceptionHandler
    {
        public int Priority => 30;
        public bool CanHandle(Exception exception) => exception is EntityNotFoundException;

        public (ProblemDetails Problem, int StatusCode) Handle(Exception exception, IHostEnvironment env, HttpContext http)
        {
            var statusCode = (int)HttpStatusCode.NotFound;
            var problem = new ProblemDetails
            {
                Status = statusCode,
                Title = "Recurso no encontrado.",
                Detail = exception.Message,
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.4",
                Instance = http.Request.Path
            };
            return (problem, statusCode);
        }
    }
}
