using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace WebGESCOMPAH.Middleware.Handlers
{
    public class ValidationExceptionHandler : IExceptionHandler
    {
        public int Priority => 10;

        public bool CanHandle(Exception exception)
            => exception is FluentValidation.ValidationException;

        public (ProblemDetails Problem, int StatusCode) Handle(Exception exception, IHostEnvironment env, HttpContext http)
        {
            var ex = (FluentValidation.ValidationException)exception;

            // Agrupa errores por propiedad
            var errors = ex.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(e => e.ErrorMessage).ToArray()
                );

            var problem = ProblemDetailsFactory.CreateValidationProblem(errors, http.Request.Path);

            return (problem, problem.Status ?? (int)HttpStatusCode.BadRequest);
        }
    }
}
