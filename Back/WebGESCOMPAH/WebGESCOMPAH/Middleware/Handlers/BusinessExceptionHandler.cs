using Microsoft.AspNetCore.Mvc;
using System.Net;
using Utilities.Exceptions;

namespace WebGESCOMPAH.Middleware.Handlers
{
    public class BusinessExceptionHandler : IExceptionHandler
    {
        public int Priority => 20;
        public bool CanHandle(Exception exception) => exception is BusinessException;

        public (ProblemDetails Problem, int StatusCode) Handle(Exception exception, IHostEnvironment env, HttpContext http)
        {
            var statusCode = (int)HttpStatusCode.Conflict; // antes: 400
            var problem = new ProblemDetails
            {
                Status = statusCode,
                Title = "Error de negocio.",
                Detail = exception.Message,
                Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.8",
                Instance = http.Request.Path
            };
            return (problem, statusCode);
        }
    }
}
