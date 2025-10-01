using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.HttpsPolicy;

namespace WebGESCOMPAH.Extensions.Infrastructure
{
    /// <summary>
    /// Configuración de compatibilidad con proxies y redirección HTTPS.
    /// </summary>
    /// <remarks>
    /// Qué hace: habilita <c>ForwardedHeaders</c> para reconocer esquema/host reales cuando la app
    /// está detrás de un proxy (p.ej., ngrok) y fija el puerto HTTPS a 443 para evitar redirecciones
    /// hacia puertos locales.
    /// 
    /// Por qué: previene bucles o URLs inconsistentes al generar enlaces (Swagger, redirects).
    /// 
    /// Para qué: que la app funcione igual en local y a través de túneles/proxy.
    /// </remarks>
    public static class ProxyHttpsExtensions
    {
        /// <summary>
        /// Registra opciones de ForwardedHeaders y HttpsRedirection.
        /// </summary>
        public static IServiceCollection AddProxyAndHttps(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<ForwardedHeadersOptions>(o =>
            {
                o.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
                o.KnownNetworks.Clear(); // DEV
                o.KnownProxies.Clear();  // DEV
            });

            services.Configure<HttpsRedirectionOptions>(o => o.HttpsPort = 443);

            return services;
        }
    }
}

