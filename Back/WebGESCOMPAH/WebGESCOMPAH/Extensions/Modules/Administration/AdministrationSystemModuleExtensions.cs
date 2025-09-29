using Business.Services.AdministrationSystem;
using Data.Services.AdministrationSystem;
using WebGESCOMPAH.Extensions.Modules.Shared;

namespace WebGESCOMPAH.Extensions.Modules.Administration
{
    /// <summary>
    /// Registro DI del módulo de Administración del Sistema.
    /// </summary>
    /// <remarks>
    /// Qué hace: registra servicios de Business.Services.AdministrationSystem y repos de
    /// Data.Services.AdministratiosSystem (se respeta la tipografía del namespace existente).
    /// Por qué: aislar el alta por feature y encapsular el typo histórico del namespace de Data.
    /// Para qué: activar funcionalidades de administración en AddApplicationServices.
    /// </remarks>
    public static class AdministrationSystemModuleExtensions
    {
        public static IServiceCollection AddAdministrationSystemModule(this IServiceCollection services)
        {
            var businessAsm = typeof(SystemParameterService).Assembly; // Business.Services.AdministrationSystem
            var dataAsm = typeof(FormModuleRepository).Assembly;       // Data.Services.AdministrationSystem

            return services.AddFeatureModule(
                businessAsm,
                dataAsm,
                "AdministrationSystem"
            );
        }
    }
}
