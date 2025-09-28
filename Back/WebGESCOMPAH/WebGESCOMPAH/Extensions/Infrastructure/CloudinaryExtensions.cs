using CloudinaryDotNet;
using Utilities.Helpers.CloudinaryHelper;

namespace WebGESCOMPAH.Extensions.Infrastructure
{
    /// <summary>
    /// Registro de servicios relacionados con Cloudinary (SDK + utilidades).
    /// </summary>
    /// <remarks>
    /// Qué hace: construye un cliente de Cloudinary a partir de configuración y lo expone como
    /// singleton, además de registrar utilidades de subida/transformación.
    /// 
    /// Por qué: centralizar la configuración y reutilizar conexiones subyacentes del SDK.
    /// 
    /// Para qué: permitir inyectar <c>Cloudinary</c> y <c>CloudinaryUtility</c> donde se necesiten.
    /// </remarks>
    public static class CloudinaryExtensions
    {
        /// <summary>
        /// Registra el cliente de Cloudinary y utilidades auxiliares.
        /// </summary>
        public static IServiceCollection AddCloudinaryServices(this IServiceCollection services, IConfiguration configuration)
        {
            var cloudinaryConfig = configuration.GetSection("Cloudinary");
            var cloudinary = new Cloudinary(new Account(
                cloudinaryConfig["CloudName"],
                cloudinaryConfig["ApiKey"],
                cloudinaryConfig["ApiSecret"]
            ));
            cloudinary.Api.Secure = true;

            services.AddSingleton(cloudinary);
            services.AddScoped<CloudinaryUtility>();

            return services;
        }
    }
}

