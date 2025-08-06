using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace WebGESCOMPAH.Middleware.Handlers
{
    public class ValidationExceptionHandler : IExceptionHandler
    {
        public bool CanHandle(Exception exception) =>
            exception is FluentValidation.ValidationException;

        public (ProblemDetails Problem, int StatusCode) Handle(Exception exception, IHostEnvironment env)
        {
            var ex = (FluentValidation.ValidationException)exception;

            var problem = new ValidationProblemDetails(
                ex.Errors
                  .GroupBy(e => e.PropertyName)
                  .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray()))
            {
                Status = (int)HttpStatusCode.BadRequest,
                Title = "Errores de validación.",
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1"
            };

            return (problem, problem.Status ?? 400);
        }
    }

}
