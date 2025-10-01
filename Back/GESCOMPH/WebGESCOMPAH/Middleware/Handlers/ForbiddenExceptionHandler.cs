using Microsoft.AspNetCore.Mvc;
using System.Net;
using WebGESCOMPH.Middleware;

namespace WebGESCOMPH.Middleware.Handlers
{
    public class ForbiddenException : Exception
    {
        public ForbiddenException(string message) : base(message) { }
    }

    public class ForbiddenExceptionHandler : IExceptionHandler
    {
        public int Priority => 35;
        public bool CanHandle(Exception exception) => exception is ForbiddenException;

        public (ProblemDetails Problem, int StatusCode) Handle(Exception exception, IHostEnvironment env, HttpContext http)
        {
            var status = (int)HttpStatusCode.Forbidden;
            var problem = new ProblemDetails
            {
                Status = status,
                Title = "Acceso prohibido.",
                Detail = exception.Message,
                Type = "https://www.rfc-editor.org/rfc/rfc7231#section-6.5.3",
                Instance = http.Request.Path
            };
            return (problem, status);
        }
    }
}
