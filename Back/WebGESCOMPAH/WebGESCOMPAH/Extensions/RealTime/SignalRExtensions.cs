using WebGESCOMPAH.RealTime;

namespace WebGESCOMPAH.Extensions.RealTime
{
    /// <summary>
    /// Registro y mapeo de hubs de SignalR de la aplicación.
    /// </summary>
    /// <remarks>
    /// Qué hace: encapsula el alta de SignalR y el mapeo de endpoints para todos los hubs.
    /// 
    /// Por qué: evitar repetición de rutas en Program y mantener un único punto de verdad.
    /// 
    /// Para qué: facilitar mantenimiento y futuras adiciones de hubs.
    /// </remarks>
    public static class SignalRExtensions
    {
        /// <summary>
        /// Registra los servicios base de SignalR.
        /// </summary>
        public static IServiceCollection AddSignalRServices(this IServiceCollection services)
        {
            services.AddSignalR();
            return services;
        }

        /// <summary>
        /// Mapea los hubs de la app bajo rutas fijas.
        /// </summary>
        public static IEndpointRouteBuilder MapAppSignalRHubs(this IEndpointRouteBuilder endpoints)
        {
            endpoints.MapHub<ContractsHub>("/api/hubs/contracts");
            endpoints.MapHub<SecurityHub>("/api/hubs/security");
            return endpoints;
        }
    }
}

