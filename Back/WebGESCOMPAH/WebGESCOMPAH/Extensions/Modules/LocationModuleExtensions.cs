using Business.Services.Location;
using Data.Services.Location;
using Microsoft.Extensions.DependencyInjection;

namespace WebGESCOMPAH.Extensions.Modules
{
    public static class LocationModuleExtensions
    {
        public static IServiceCollection AddLocationModule(this IServiceCollection services)
        {
            var businessAsm = typeof(CityService).Assembly; // Business.Services.Location
            var dataAsm = typeof(CityRepository).Assembly;   // Data.Services.Location

            services.Scan(scan => scan
                .FromAssemblies(businessAsm, dataAsm)
                .AddClasses(c => c.Where(t => t.Namespace != null && t.Namespace.Contains("Services.Location") && !t.Name.EndsWith("Repository")))
                    .AsImplementedInterfaces()
                    .WithScopedLifetime()
                .AddClasses(c => c.Where(t => t.Namespace != null && t.Namespace.Contains("Services.Location") && t.Name.EndsWith("Repository")))
                    .AsMatchingInterface()
                    .WithScopedLifetime()
            );

            return services;
        }
    }
}

