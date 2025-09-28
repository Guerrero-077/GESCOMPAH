using Business.Services.SecurityAuthentication;
using Data.Services.SecurityAuthentication;
using Microsoft.Extensions.DependencyInjection;

namespace WebGESCOMPAH.Extensions.Modules
{
    public static class SecurityAuthenticationModuleExtensions
    {
        public static IServiceCollection AddSecurityAuthenticationModule(this IServiceCollection services)
        {
            var businessAsm = typeof(AuthService).Assembly; // Business.Services.SecurityAuthentication
            var dataAsm = typeof(UserRepository).Assembly;   // Data.Services.SecurityAuthentication

            services.Scan(scan => scan
                .FromAssemblies(businessAsm, dataAsm)
                .AddClasses(c => c.Where(t => t.Namespace != null && t.Namespace.Contains("Services.SecurityAuthentication") && !t.Name.EndsWith("Repository")))
                    .AsImplementedInterfaces()
                    .WithScopedLifetime()
                .AddClasses(c => c.Where(t => t.Namespace != null && t.Namespace.Contains("Services.SecurityAuthentication") && t.Name.EndsWith("Repository")))
                    .AsMatchingInterface()
                    .WithScopedLifetime()
            );

            return services;
        }
    }
}
