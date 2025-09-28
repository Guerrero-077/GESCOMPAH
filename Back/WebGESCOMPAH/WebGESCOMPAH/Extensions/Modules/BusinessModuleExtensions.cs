using Business.Services.Business;
using Data.Services.Business;
using Microsoft.Extensions.DependencyInjection;

namespace WebGESCOMPAH.Extensions.Modules
{
    public static class BusinessModuleExtensions
    {
        public static IServiceCollection AddBusinessModule(this IServiceCollection services)
        {
            var businessAsm = typeof(ContractService).Assembly; // Business.Services.Business
            var dataAsm = typeof(ContractRepository).Assembly;   // Data.Services.Business

            services.Scan(scan => scan
                .FromAssemblies(businessAsm, dataAsm)
                .AddClasses(c => c.Where(t => t.Namespace != null && t.Namespace.Contains("Services.Business") && !t.Name.EndsWith("Repository")))
                    .AsImplementedInterfaces()
                    .WithScopedLifetime()
                .AddClasses(c => c.Where(t => t.Namespace != null && t.Namespace.Contains("Services.Business") && t.Name.EndsWith("Repository")))
                    .AsMatchingInterface()
                    .WithScopedLifetime()
            );

            return services;
        }
    }
}

