using Business.Services.Location;
using Data.Services.Location;
using WebGESCOMPAH.Extensions.Modules.Shared;

namespace WebGESCOMPAH.Extensions.Modules.Location
{
    /// <summary>
    /// Registro DI del módulo de Localización (Ciudades, Departamentos).
    /// </summary>
    /// <remarks>
    /// Registra servicios/repos bajo Services.Location por convención, vía AddFeatureModule.
    /// </remarks>
    public static class LocationModuleExtensions
    {
        public static IServiceCollection AddLocationModule(this IServiceCollection services)
        {
            var businessAsm = typeof(CityService).Assembly; // Business.Services.Location
            var dataAsm = typeof(CityRepository).Assembly;   // Data.Services.Location

            return services.AddFeatureModule(businessAsm, dataAsm, "Location");
        }
    }
}
