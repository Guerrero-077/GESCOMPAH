using Business.Services.Utilities;
using Data.Services.Utilities;
using WebGESCOMPAH.Extensions.Modules.Shared;

namespace WebGESCOMPAH.Extensions.Modules.Utilities
{
    /// <summary>
    /// Registro DI del módulo de Utilidades (imágenes, etc.).
    /// </summary>
    /// <remarks>
    /// Registra servicios/repos bajo Services.Utilities de manera automática.
    /// </remarks>
    public static class UtilitiesModuleExtensions
    {
        public static IServiceCollection AddUtilitiesModule(this IServiceCollection services)
        {
            var businessAsm = typeof(ImageService).Assembly; // Business.Services.Utilities
            var dataAsm = typeof(ImagesRepository).Assembly; // Data.Services.Utilities

            return services.AddFeatureModule(businessAsm, dataAsm, "Utilities");
        }
    }
}
