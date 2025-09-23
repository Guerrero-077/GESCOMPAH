using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace WebGESCOMPAH.Middleware
{
    public static class ProblemDetailsFactory
    {
        public static ProblemDetails Create(
            int statusCode,
            string title,
            string detail,
            string type,
            string instance)
        {
            return new ProblemDetails
            {
                Status = statusCode,
                Title = title,
                Detail = detail,
                Type = type,
                Instance = instance
            };
        }

        public static ValidationProblemDetails CreateValidationProblem(
            IDictionary<string, string[]> errors,
            string instance)
        {
            return new ValidationProblemDetails(errors)
            {
                Status = (int)HttpStatusCode.BadRequest,
                Title = "Errores de validación.",
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                Instance = instance
            };
        }
    }
}
