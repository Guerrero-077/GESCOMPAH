using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace WebGESCOMPAH.Middleware.Handlers
{
    public class ForbiddenException : Exception
    {
        public ForbiddenException(string message) : base(message) { }
    }

    public class ForbiddenExceptionHandler : IExceptionHandler
    {
        public int Priority => 35;

        public bool CanHandle(Exception exception)
            => exception is ForbiddenException;

        public (ProblemDetails Problem, int StatusCode) Handle(Exception exception, IHostEnvironment env, HttpContext http)
        {
            var statusCode = (int)HttpStatusCode.Forbidden;

            var problem = ProblemDetailsFactory.Create(
                statusCode,
                title: "Acceso prohibido.",
                detail: exception.Message,
                type: "https://www.rfc-editor.org/rfc/rfc7231#section-6.5.3",
                instance: http.Request.Path
            );

            return (problem, statusCode);
        }
    }
}
