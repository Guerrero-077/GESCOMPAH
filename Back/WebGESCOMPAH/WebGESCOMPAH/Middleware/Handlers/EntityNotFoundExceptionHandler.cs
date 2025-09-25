using Microsoft.AspNetCore.Mvc;
using System.Net;
using Utilities.Exceptions;

namespace WebGESCOMPAH.Middleware.Handlers
{
    public class EntityNotFoundExceptionHandler : IExceptionHandler
    {
        public int Priority => 15; // después de FluentValidation (10) y antes de Business genérico (20)

        public bool CanHandle(Exception exception)
            => exception is EntityNotFoundException;

        public (ProblemDetails Problem, int StatusCode) Handle(Exception exception, IHostEnvironment env, HttpContext http)
        {
            var notFound = (EntityNotFoundException)exception;
            var statusCode = (int)HttpStatusCode.NotFound;

            var problem = ProblemDetailsFactory.Create(
                statusCode,
                title: "Recurso no encontrado.",
                detail: notFound.Message,
                type: "https://tools.ietf.org/html/rfc7231#section-6.5.4",
                instance: http.Request.Path
            );

            // info opcional
            problem.Extensions ??= new Dictionary<string, object?>();
            problem.Extensions["entityType"] = notFound.EntityType;
            problem.Extensions["entityId"] = notFound.EntityId;

            return (problem, statusCode);
        }
    }
}

