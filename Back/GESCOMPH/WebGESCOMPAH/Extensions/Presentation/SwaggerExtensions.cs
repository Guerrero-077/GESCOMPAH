using Microsoft.OpenApi.Models;

namespace WebGESCOMPAH.Extensions.Presentation
{
    /// <summary>
    /// Configuración de Swagger/OpenAPI con soporte para proxies inversos.
    /// </summary>
    /// <remarks>
    /// Qué hace: registra generador de Swagger y en ejecución reescribe dinámicamente el servidor
    /// para reflejar <c>X-Forwarded-Proto</c> y <c>X-Forwarded-Host</c>, evitando URLs incorrectas
    /// en entornos tras proxy (ngrok, Nginx, etc.).
    /// 
    /// Por qué: sin esto, la UI puede apuntar a host/puerto equivocados rompiendo pruebas/compartidos.
    /// 
    /// Para qué: documentación interactiva estable tanto en local como publicado.
    /// </remarks>
    public static class SwaggerExtensions
    {
        /// <summary>
        /// Registra servicios de Swagger (explorer + generator).
        /// </summary>
        public static IServiceCollection AddCustomSwagger(this IServiceCollection services)
        {
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
            return services;
        }

        /// <summary>
        /// Habilita Swagger y ajusta el server según headers reenviados por el proxy.
        /// </summary>
        public static IApplicationBuilder UseCustomSwagger(this IApplicationBuilder app)
        {
            app.UseSwagger(c =>
            {
                c.PreSerializeFilters.Add((swagger, req) =>
                {
                    var scheme = req.Headers["X-Forwarded-Proto"].FirstOrDefault() ?? req.Scheme;
                    var host = req.Headers["X-Forwarded-Host"].FirstOrDefault() ?? req.Host.Value;
                    var basePath = req.PathBase.HasValue ? req.PathBase.Value : string.Empty;

                    swagger.Servers = new List<OpenApiServer> {
                        new OpenApiServer { Url = $"{scheme}://{host}{basePath}" }
                    };
                });
            });

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "GESCOMPH API v1");
                c.RoutePrefix = "swagger";
            });

            return app;
        }
    }
}

