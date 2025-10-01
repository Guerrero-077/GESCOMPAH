using Business.Services.SecurityAuthentication;
using Data.Services.SecurityAuthentication;
using WebGESCOMPAH.Extensions.Modules.Shared;

namespace WebGESCOMPAH.Extensions.Modules.Security
{
    /// <summary>
    /// Registro DI del módulo de Seguridad/Autenticación.
    /// </summary>
    /// <remarks>
    /// Incluye servicios de autenticación y repositorios tanto en Services.SecurityAuthentication como
    /// en Repositories.Implementations.SecurityAuthentication.
    /// </remarks>
    public static class SecurityAuthenticationModuleExtensions
    {
        public static IServiceCollection AddSecurityAuthenticationModule(this IServiceCollection services)
        {
            var businessAsm = typeof(AuthService).Assembly; // Business.Services.SecurityAuthentication
            var dataAsm = typeof(UserRepository).Assembly;   // Data.Services.SecurityAuthentication

            // Acepta repos en Services.SecurityAuthentication y también en Repositories.Implementations.SecurityAuthentication
            return services.AddFeatureModule(
                businessAsm,
                dataAsm,
                "SecurityAuthentication",
                "Repositories.Implementations.SecurityAuthentication"
            );
        }
    }
}
