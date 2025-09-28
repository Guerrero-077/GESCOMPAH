using Business.Interfaces.Notifications;
using Microsoft.Extensions.DependencyInjection;
using WebGESCOMPAH.RealTime;

namespace WebGESCOMPAH.Extensions.Modules
{
    public static class NotificationsModuleExtensions
    {
        public static IServiceCollection AddNotificationsModule(this IServiceCollection services)
        {
            services.AddScoped<IContractNotificationService, SignalRContractNotificationService>();
            services.AddScoped<IPermissionsNotificationService, SignalRPermissionsNotificationService>();
            return services;
        }
    }
}

