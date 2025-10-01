using Business.Interfaces.PDF;
using Business.Services.Utilities.PDF;
using Entity.Domain.Models.Implements.SecurityAuthentication;
using Entity.DTOs.Validations.SecurityAuthentication.Auth;
using FluentValidation;
using FluentValidation.AspNetCore;
using WebGESCOMPAH.Extensions.Presentation;
using WebGESCOMPAH.Extensions.RealTime;
using WebGESCOMPAH.Extensions.Validation;
using WebGESCOMPAH.RealTime;

namespace WebGESCOMPAH.Extensions.Infrastructure
{
    /// <summary>
    /// Paquete de infraestructura transversal para la WebApp.
    /// </summary>
    /// <remarks>
    /// Qué hace: agrupa el registro de piezas no estrictamente de dominio: CORS, autenticación JWT
    /// y cookies, acceso a datos (DbContext), validaciones, ProblemDetails, SignalR, Cloudinary,
    /// Hangfire y compatibilidad proxy/HTTPS, además de dependencias de warmup PDF y jobs.
    /// 
    /// Por qué: separar claramente configuración de infraestructura de los módulos de negocio
    /// (que se registran en <c>AddApplicationServices()</c>), manteniendo <c>Program.cs</c> legible.
    /// 
    /// Para qué: punto único y coherente para infraestructura cross‑cutting.
    /// </remarks>
    public static class InfrastructureService
    {
        /// <summary>
        /// Registra servicios transversales de infraestructura.
        /// </summary>
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            // CORS
            services.AddCustomCors(configuration);

            // JWT + Cookies
            services.Configure<JwtSettings>(configuration.GetSection("Jwt"));
            services.Configure<CookieSettings>(configuration.GetSection("Cookie"));
            services.AddJwtAuthentication(configuration);

            // DB (multi-provider)
            services.AddDatabase(configuration);

            // Validaciones + auto-validación
            services.AddValidatorsFromAssemblyContaining<RegisterDtoValidator>();
            services.AddFluentValidationAutoValidation();
            services.AddValidationProblemDetailsResponse();

            // SignalR
            services.AddSignalRServices();

            // Cloudinary
            services.AddCloudinaryServices(configuration);

            // Hangfire
            services.AddHangfireServices(configuration);

            // Proxy/HTTPS (ngrok)
            services.AddProxyAndHttps(configuration);

            // Otros servicios transversales
            services.AddScoped<IContractPdfGeneratorService, ContractPdfService>();
            services.AddScoped<ObligationJobs>();
            services.AddScoped<ContractJobs>();

            return services;
        }
    }
}
