using Microsoft.Extensions.DependencyInjection;

namespace WebGESCOMPAH.Extensions.Modules
{
    public static class PersonsModuleExtensions
    {
        public static IServiceCollection AddPersonsModule(this IServiceCollection services)
        {
            // Business.Services.Persons + Data.Services.Persons (si existen)
            services.Scan(scan => scan
                .FromApplicationDependencies(a => a.GetName().Name == "Business" || a.GetName().Name == "Data")
                .AddClasses(c => c.Where(t => t.Namespace != null && t.Namespace.Contains("Services.Persons") && !t.Name.EndsWith("Repository")))
                    .AsImplementedInterfaces()
                    .WithScopedLifetime()
                .AddClasses(c => c.Where(t => t.Namespace != null && t.Namespace.Contains("Services.Persons") && t.Name.EndsWith("Repository")))
                    .AsMatchingInterface()
                    .WithScopedLifetime()
            );

            return services;
        }
    }
}

