using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Net;
using WebGESCOMPH.Middleware;

namespace WebGESCOMPH.Middleware.Handlers
{
    public class SecurityTokenExceptionHandler : IExceptionHandler
    {
        public int Priority => 35; // Entre UnauthorizedAccess (40) y Validation (10)
        public bool CanHandle(Exception exception) => exception is SecurityTokenException;

        public (ProblemDetails Problem, int StatusCode) Handle(Exception exception, IHostEnvironment env, HttpContext http)
        {
            var statusCode = (int)HttpStatusCode.Unauthorized;
            var problem = new ProblemDetails
            {
                Status = statusCode,
                Title = "Token inválido o expirado.",
                Detail = string.IsNullOrWhiteSpace(exception.Message)
                    ? "El refresh token es inválido o ha expirado."
                    : exception.Message,
                Type = "https://datatracker.ietf.org/doc/html/rfc7235#section-3.1",
                Instance = http.Request.Path
            };
            return (problem, statusCode);
        }
    }
}

