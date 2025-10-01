using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace WebGESCOMPAH.Middleware.Handlers
{
    public class DbUpdateExceptionHandler : IExceptionHandler
    {
        public int Priority => 46;

        public bool CanHandle(Exception exception)
            => exception is DbUpdateException;

        public (ProblemDetails Problem, int StatusCode) Handle(Exception exception, IHostEnvironment env, HttpContext http)
        {
            var statusCode = (int)HttpStatusCode.Conflict;

            var problem = ProblemDetailsFactory.Create(
                statusCode,
                title: "Error de base de datos.",
                detail: "Violación de unicidad o restricción de base de datos.",
                type: "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.8",
                instance: http.Request.Path
            );

            return (problem, statusCode);
        }
    }
}
