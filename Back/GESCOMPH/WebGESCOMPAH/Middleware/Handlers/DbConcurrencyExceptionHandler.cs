using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using WebGESCOMPH.Middleware;

namespace WebGESCOMPH.Middleware.Handlers
{
    public class DbConcurrencyExceptionHandler : IExceptionHandler
    {
        public int Priority => 45;
        public bool CanHandle(Exception exception) => exception is DbUpdateConcurrencyException;

        public (ProblemDetails Problem, int StatusCode) Handle(Exception exception, IHostEnvironment env, HttpContext http)
        {
            var status = (int)HttpStatusCode.Conflict;
            var problem = new ProblemDetails
            {
                Status = status,
                Title = "Conflicto de concurrencia.",
                Detail = "El recurso fue modificado por otro proceso. Refresca y reintenta.",
                Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.8",
                Instance = http.Request.Path
            };
            return (problem, status);
        }
    }
}
