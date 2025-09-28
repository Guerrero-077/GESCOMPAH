using Entity.Infrastructure.Binder;
using System.Text.Json;
using System.Text.Json.Serialization;
using WebGESCOMPAH.Filters;

namespace WebGESCOMPAH.Extensions.Presentation
{
    /// <summary>
    /// Configuración de controladores y serialización para la capa Web/API.
    /// </summary>
    /// <remarks>
    /// Qué hace: registra MVC controllers, inserta un binder flexible para decimales,
    /// añade un filtro global que emite headers de paginación y ajusta JSON para ser tolerante
    /// con números en string, comas finales y comentarios.
    /// 
    /// Por qué: mejora DX/UX del cliente (inputs flexibles) y estandariza la salida (paginación).
    /// 
    /// Para qué: mantener Program.cs limpio y centralizar la configuración de presentación.
    /// </remarks>
    public static class ControllersExtensions
    {
        /// <summary>
        /// Agrega controladores y ajusta opciones de JSON/binders/filtros globales.
        /// </summary>
        public static IServiceCollection AddPresentationControllers(this IServiceCollection services)
        {
            services
                .AddControllers(options =>
                {
                    options.ModelBinderProviders.Insert(0, new FlexibleDecimalModelBinderProvider());
                    // Filtro global: headers de paginación
                    options.Filters.Add(new PagedResultHeadersFilter());
                })
                .AddJsonOptions(o =>
                {
                    o.JsonSerializerOptions.NumberHandling = JsonNumberHandling.AllowReadingFromString;
                    o.JsonSerializerOptions.AllowTrailingCommas = true;
                    o.JsonSerializerOptions.ReadCommentHandling = JsonCommentHandling.Skip;
                });

            return services;
        }
    }
}

