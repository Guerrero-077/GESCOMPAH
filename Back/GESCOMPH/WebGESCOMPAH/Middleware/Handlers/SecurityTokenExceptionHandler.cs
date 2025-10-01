using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Net;

namespace WebGESCOMPAH.Middleware.Handlers
{
    public class SecurityTokenExceptionHandler : IExceptionHandler
    {
        public int Priority => 35;

        public bool CanHandle(Exception exception)
            => exception is SecurityTokenException;

        public (ProblemDetails Problem, int StatusCode) Handle(Exception exception, IHostEnvironment env, HttpContext http)
        {
            var statusCode = (int)HttpStatusCode.Unauthorized;

            var detail = string.IsNullOrWhiteSpace(exception.Message)
                ? "El refresh token es inválido o ha expirado."
                : exception.Message;

            var problem = ProblemDetailsFactory.Create(
                statusCode,
                title: "Token inválido o expirado.",
                detail: detail,
                type: "https://datatracker.ietf.org/doc/html/rfc7235#section-3.1",
                instance: http.Request.Path
            );

            return (problem, statusCode);
        }
    }
}
