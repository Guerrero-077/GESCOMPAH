using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace WebGESCOMPAH.Middleware.Handlers
{
    public class TimeoutExceptionHandler : IExceptionHandler
    {
        public int Priority => 48;

        public bool CanHandle(Exception exception)
            => exception is TimeoutException || exception is TaskCanceledException;

        public (ProblemDetails Problem, int StatusCode) Handle(Exception exception, IHostEnvironment env, HttpContext http)
        {
            var statusCode = (int)HttpStatusCode.GatewayTimeout;

            var detail = "El servidor no respondió a tiempo.";

            var problem = ProblemDetailsFactory.Create(
                statusCode,
                title: "Tiempo de espera agotado.",
                detail: detail,
                type: "https://tools.ietf.org/html/rfc7231#section-6.6.5",
                instance: http.Request.Path
            );

            return (problem, statusCode);
        }
    }
}
