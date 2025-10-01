using Microsoft.AspNetCore.Mvc;

namespace WebGESCOMPAH.Middleware.Handlers
{
    public class RateLimitException : Exception
    {
        public RateLimitException(string message) : base(message) { }
    }

    public class RateLimitExceptionHandler : IExceptionHandler
    {
        public int Priority => 15;

        public bool CanHandle(Exception exception) => exception is RateLimitException;

        public (ProblemDetails Problem, int StatusCode) Handle(Exception exception, IHostEnvironment env, HttpContext http)
        {
            var statusCode = 429;

            var problem = ProblemDetailsFactory.Create(
                statusCode,
                title: "Demasiadas solicitudes.",
                detail: exception.Message,
                type: "https://www.rfc-editor.org/rfc/rfc6585#section-4",
                instance: http.Request.Path
            );

            return (problem, statusCode);
        }
    }
}
