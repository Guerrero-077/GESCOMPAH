using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace WebGESCOMPAH.Middleware.Handlers
{
    public class DbConcurrencyExceptionHandler : IExceptionHandler
    {
        public int Priority => 45;

        public bool CanHandle(Exception exception)
            => exception is DbUpdateConcurrencyException;

        public (ProblemDetails Problem, int StatusCode) Handle(Exception exception, IHostEnvironment env, HttpContext http)
        {
            var statusCode = (int)HttpStatusCode.Conflict;

            var problem = ProblemDetailsFactory.Create(
                statusCode,
                title: "Conflicto de concurrencia.",
                detail: "El recurso fue modificado por otro proceso. Refresca y reintenta.",
                type: "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.8",
                instance: http.Request.Path
            );

            return (problem, statusCode);
        }
    }
}
