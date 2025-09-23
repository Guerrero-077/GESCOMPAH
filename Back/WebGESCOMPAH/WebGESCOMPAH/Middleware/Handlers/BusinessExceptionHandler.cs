using Microsoft.AspNetCore.Mvc;
using System.Net;
using Utilities.Exceptions;

namespace WebGESCOMPAH.Middleware.Handlers
{
    public class BusinessExceptionHandler : IExceptionHandler
    {
        public int Priority => 20;

        public bool CanHandle(Exception exception)
            => exception is BusinessException;

        public (ProblemDetails Problem, int StatusCode) Handle(Exception exception, IHostEnvironment env, HttpContext http)
        {
            var statusCode = (int)HttpStatusCode.UnprocessableEntity;

            var problem = ProblemDetailsFactory.Create(
                statusCode,
                title: "Error de negocio.",
                detail: exception.Message,
                type: "https://datatracker.ietf.org/doc/html/rfc4918#section-11.2",
                instance: http.Request.Path
            );

            return (problem, statusCode);
        }
    }
}
