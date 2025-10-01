using Microsoft.AspNetCore.Mvc;
using System.Net;
using WebGESCOMPH.Middleware;

namespace WebGESCOMPH.Middleware.Handlers
{
    public class HttpRequestExceptionHandler : IExceptionHandler
    {
        public int Priority => 47;
        public bool CanHandle(Exception exception) => exception is HttpRequestException;

        public (ProblemDetails Problem, int StatusCode) Handle(Exception exception, IHostEnvironment env, HttpContext http)
        {
            var status = (int)HttpStatusCode.BadGateway;
            var problem = new ProblemDetails
            {
                Status = status,
                Title = "Error al comunicarse con servicio externo.",
                Detail = env.IsDevelopment() ? exception.ToString() : "Fallo la comunicación con un servicio externo.",
                Type = "https://www.rfc-editor.org/rfc/rfc7231#section-6.6.3",
                Instance = http.Request.Path
            };
            return (problem, status);
        }
    }
}
