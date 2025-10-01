using Microsoft.AspNetCore.Mvc;

namespace WebGESCOMPAH.Extensions.Presentation
{
    /// <summary>
    /// Respuesta estándar de validación (RFC 7807) para solicitudes inválidas.
    /// </summary>
    /// <remarks>
    /// Qué hace: reemplaza la respuesta por defecto de <c>[ApiController]</c> cuando el modelo
    /// no es válido, retornando un <c>ValidationProblemDetails</c> con metadatos útiles y un
    /// <c>traceId</c> para correlación.
    /// 
    /// Por qué: dar mensajes consistentes y ricos a frontend/consumidores.
    /// 
    /// Para qué: facilitar depuración y UX al integrar con la API.
    /// </remarks>
    public static class ProblemDetailsExtensions
    {
        /// <summary>
        /// Configura una respuesta estándar (Problem Details) para errores de validación (400) de ModelState.
        /// </summary>
        public static IServiceCollection AddValidationProblemDetailsResponse(this IServiceCollection services)
        {
            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = ctx =>
                {
                    var problem = new ValidationProblemDetails(ctx.ModelState)
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Title = "Errores de validación en la solicitud.",
                        Detail = "Revisa el formato del JSON y los campos obligatorios (evita comas finales y usa fechas YYYY-MM-DD).",
                        Type = "https://tools.ietf.org/html/rfc9110#section-15.5.1",
                        Instance = ctx.HttpContext.Request.Path
                    };
                    problem.Extensions["traceId"] = ctx.HttpContext.TraceIdentifier;
                    return new BadRequestObjectResult(problem);
                };
            });

            return services;
        }
    }
}

