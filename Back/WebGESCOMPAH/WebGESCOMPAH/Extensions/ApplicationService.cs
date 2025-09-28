using Microsoft.Extensions.DependencyInjection;
using WebGESCOMPAH.Extensions.Modules;

namespace WebGESCOMPAH.Extensions
{
    public static class ApplicationService
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // Núcleo cross-cutting
            services.AddCoreModule();

            // Registro modular por features
            services
                .AddSecurityAuthenticationModule()
                .AddBusinessModule()
                .AddLocationModule()
                .AddAdministrationSystemModule()
                .AddUtilitiesModule()
                .AddPersonsModule()
                .AddExceptionHandlersModule();

            // Notificaciones (SignalR en Web)
            services.AddNotificationsModule();

            return services;
        }
    }
}
