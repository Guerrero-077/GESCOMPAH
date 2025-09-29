using WebGESCOMPAH.Extensions.Modules.Administration;
using WebGESCOMPAH.Extensions.Modules.Business;
using WebGESCOMPAH.Extensions.Modules.Core;
using WebGESCOMPAH.Extensions.Modules.Exceptions;
using WebGESCOMPAH.Extensions.Modules.Location;
using WebGESCOMPAH.Extensions.Modules.Notifications;
using WebGESCOMPAH.Extensions.Modules.Persons;
using WebGESCOMPAH.Extensions.Modules.Security;
using WebGESCOMPAH.Extensions.Modules.Utilities;

namespace WebGESCOMPAH.Extensions.Composition
{
    /// <summary>
    /// Ensambla los módulos de la aplicación (dominio) en el contenedor DI.
    /// </summary>
    /// <remarks>
    /// Qué hace: agrega Core + módulos por feature (Seguridad, Business, Location, AdminSystem,
    /// Utilities, Persons) y el módulo de excepciones y notificaciones.
    /// 
    /// Por qué: separar el registro de servicios del dominio de la infraestructura y presentación.
    /// 
    /// Para qué: punto único para activar/desactivar módulos y mantener Program.cs minimalista.
    /// </remarks>
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

