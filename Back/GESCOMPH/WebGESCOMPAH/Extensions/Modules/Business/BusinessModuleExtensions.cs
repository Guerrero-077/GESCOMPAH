using Business.Services.Business;
using Data.Services.Business;
using WebGESCOMPAH.Extensions.Modules.Shared;

namespace WebGESCOMPAH.Extensions.Modules.Business
{
    /// <summary>
    /// Registro DI del módulo de negocio (contratos, citas, plazas, etc.).
    /// </summary>
    /// <remarks>
    /// Qué hace: usa AddFeatureModule para registrar servicios y repos bajo Services.Business.
    /// Por qué: mantener una convención DRY y robusta en el armado de módulos.
    /// Para qué: habilitar todas las capacidades de dominio de negocio.
    /// </remarks>
    public static class BusinessModuleExtensions
    {
        public static IServiceCollection AddBusinessModule(this IServiceCollection services)
        {
            var businessAsm = typeof(ContractService).Assembly; // Business.Services.Business
            var dataAsm = typeof(ContractRepository).Assembly;   // Data.Services.Business

            return services.AddFeatureModule(businessAsm, dataAsm, "Business");
        }
    }
}
