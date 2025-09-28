using Business.Services.AdministrationSystem;
using Data.Services.AdministratiosSystem;
using Microsoft.Extensions.DependencyInjection;

namespace WebGESCOMPAH.Extensions.Modules
{
    public static class AdministrationSystemModuleExtensions
    {
        public static IServiceCollection AddAdministrationSystemModule(this IServiceCollection services)
        {
            // En Data el namespace es 'Data.Services.AdministratiosSystem' (ojo con la ortografía)
            var businessAsm = typeof(SystemParameterService).Assembly; // Business.Services.AdministrationSystem
            var dataAsm = typeof(FormModuleRepository).Assembly;       // Data.Services.AdministratiosSystem

            services.Scan(scan => scan
                .FromAssemblies(businessAsm)
                .AddClasses(c => c.Where(t => t.Namespace != null && t.Namespace.Contains("Services.AdministrationSystem") && !t.Name.EndsWith("Repository")))
                    .AsImplementedInterfaces()
                    .WithScopedLifetime()
            );

            services.Scan(scan => scan
                .FromAssemblies(dataAsm)
                .AddClasses(c => c.Where(t => t.Namespace != null && t.Namespace.Contains("Services.AdministratiosSystem") && t.Name.EndsWith("Repository")))
                    .AsMatchingInterface()
                    .WithScopedLifetime()
            );

            return services;
        }
    }
}
