using Business.CustomJWT;
using Business.Interfaces;
using Business.Mapping;
using Business.Repository;
using Data.Interfaz.DataBasic;
using Data.Repository;
using Mapster;
using Utilities.Messaging.Factories;
using Utilities.Messaging.Implements;
using Utilities.Messaging.Interfaces;
using WebGESCOMPAH.Infrastructure;

namespace WebGESCOMPAH.Extensions.Modules.Core
{
    /// <summary>
    /// Registro DI del núcleo compartido (infra común, Mapster, UoW, JWT, CurrentUser, etc.).
    /// </summary>
    /// <remarks>
    /// Separa dependencias base que todos los módulos pueden requerir, manteniendo consistencia.
    /// </remarks>
    public static class CoreModuleExtensions
    {
        public static IServiceCollection AddCoreModule(this IServiceCollection services)
        {
            // Mensajería (factoría + fachada)
            services.AddSingleton<IEmailServiceFactory, EmailServiceFactory>();
            services.AddTransient<ISendCode, EmailService>();

            // Data genérica + UoW
            services.AddScoped(typeof(IDataGeneric<>), typeof(DataGeneric<>));
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // JWT, CurrentUser, Cookie factory
            services.AddScoped<IToken, TokenBusiness>();
            services.AddSingleton<IAuthCookieFactory, AuthCookieFactory>();
            services.AddScoped<Microsoft.AspNetCore.Identity.IPasswordHasher<Entity.Domain.Models.Implements.SecurityAuthentication.User>, Microsoft.AspNetCore.Identity.PasswordHasher<Entity.Domain.Models.Implements.SecurityAuthentication.User>>();

            // Infraestructura común
            services.AddMemoryCache();
            services.AddHttpContextAccessor();
            services.AddScoped<ICurrentUser, CurrentUser>();

            // Middleware base
            services.AddTransient<ExceptionMiddleware>();

            // Mapster (configuración global)
            services.AddMapster();
            MapsterConfig.Register();

            return services;
        }
    }
}
