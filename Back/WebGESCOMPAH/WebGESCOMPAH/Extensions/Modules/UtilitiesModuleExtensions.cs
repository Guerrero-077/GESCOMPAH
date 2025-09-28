using Business.Services.Utilities;
using Data.Services.Utilities;
using Microsoft.Extensions.DependencyInjection;

namespace WebGESCOMPAH.Extensions.Modules
{
    public static class UtilitiesModuleExtensions
    {
        public static IServiceCollection AddUtilitiesModule(this IServiceCollection services)
        {
            var businessAsm = typeof(ImageService).Assembly; // Business.Services.Utilities
            var dataAsm = typeof(ImagesRepository).Assembly; // Data.Services.Utilities

            services.Scan(scan => scan
                .FromAssemblies(businessAsm, dataAsm)
                .AddClasses(c => c.Where(t => t.Namespace != null && t.Namespace.Contains("Services.Utilities") && !t.Name.EndsWith("Repository")))
                    .AsImplementedInterfaces()
                    .WithScopedLifetime()
                .AddClasses(c => c.Where(t => t.Namespace != null && t.Namespace.Contains("Services.Utilities") && t.Name.EndsWith("Repository")))
                    .AsMatchingInterface()
                    .WithScopedLifetime()
            );

            return services;
        }
    }
}

