using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace WebGESCOMPAH.Middleware.Handlers
{
    public class DefaultExceptionHandler : IExceptionHandler
    {
        public int Priority => int.MaxValue;
        public bool CanHandle(Exception exception) => true;

        public (ProblemDetails Problem, int StatusCode) Handle(Exception exception, IHostEnvironment env, HttpContext http)
        {
            var status = (int)HttpStatusCode.InternalServerError;
            var problem = new ProblemDetails
            {
                Status = status,
                Title = "Error interno del servidor.",
                Detail = env.IsDevelopment() ? exception.ToString() : "Ocurrió un error inesperado.",
                Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1",
                Instance = http.Request.Path
            };
            return (problem, status);
        }
    }
}
