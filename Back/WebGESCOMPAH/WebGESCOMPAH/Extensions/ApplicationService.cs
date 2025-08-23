using Business.CustomJWT;
using Business.Interfaces;
using Business.Interfaces.Implements.AdministrationSystem;
using Business.Interfaces.Implements.Business;
using Business.Interfaces.Implements.Location;
using Business.Interfaces.Implements.Persons;
using Business.Interfaces.Implements.SecrutityAuthentication;
using Business.Interfaces.Implements.Utilities;
using Business.Mapping;
using Business.Services.AdministrationSystem;
using Business.Services.Business;
using Business.Services.Location;
using Business.Services.SecrutityAuthentication;
using Business.Services.Utilities;
using Data.Interfaz.DataBasic;
using Data.Interfaz.IDataImplemenent.AdministrationSystem;
using Data.Interfaz.IDataImplemenent.Business;
using Data.Interfaz.IDataImplemenent.Location;
using Data.Interfaz.IDataImplemenent.Persons;
using Data.Interfaz.IDataImplemenent.SecurityAuthentication;
using Data.Interfaz.IDataImplemenent.Utilities;
using Data.Interfaz.Security;
using Data.Repository;
using Data.Services.AdministratiosSystem;
using Data.Services.Business;
using Data.Services.Location;
using Data.Services.Persons;
using Data.Services.SecurityAuthentication;
using Data.Services.Utilities;
using Mapster;
using Utilities.Messaging.Implements;
using Utilities.Messaging.Interfaces;
using WebGESCOMPAH.Infrastructure;
using WebGESCOMPAH.Middleware;
using WebGESCOMPAH.Middleware.Handlers;

namespace WebGESCOMPAH.Extensions
{
    public static class ApplicationService
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {


            services.AddScoped<IUserMeRepository, MeRepository>();

            //Email
            services.AddTransient<ISendCode, EmailService>();

            //Auth
            services.AddScoped<IPasswordResetCodeRepository, PasswordResetCodeRepository>();
            //services.AddScoped<EncriptePassword>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IUserContextService, UserContextService>();

            //Services
            services.AddScoped<IPersonService, PersonService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IRolService, RolService>();
            services.AddScoped<IPlazaService, PlazasService>();
            services.AddScoped<IEstablishmentService, EstablishmentService>();
            services.AddScoped<IImagesService, ImageService>();
            services.AddScoped<IAppointmentService, AppointmentService>();
            services.AddScoped<IRolFormPermissionService, RolFormPermissionService>();
            services.AddScoped<IFormMouduleService, FormModuleService>();
            services.AddScoped<IModuleService, ModuleService>();
            services.AddScoped<IRolUserService, RolUserService>();
            services.AddScoped<IFormService, FormService>();
            services.AddScoped<IPermissionService, PermissionService>();
            services.AddScoped<ISystemParameterService, SystemParameterService>();


            //Mapping
            services.AddMapster();
            MapsterConfig.Register();

            //services Data
            services.AddScoped(typeof(IDataGeneric<>), typeof(DataGeneric<>));

            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IRolFormPermissionRepository, RolFormPermissionRepository>();


            services.AddScoped<IEstablishmentsRepository, EstablishmentsRepository>();
            services.AddScoped<IImagesRepository, ImagesRepository>();
            services.AddScoped<ICityRepository, CityRepository>();
            services.AddScoped<ICityService, CityService>();
            services.AddScoped<IDepartmentService, DepartmentService>();
            services.AddScoped<IAppointmentRepository, AppointmentRepository>();
            services.AddScoped<IFormModuleRepository, FormModuleRepository>();
            services.AddScoped<IRolUserRepository,  RolUserRepository>();
            services.AddScoped<IPersonRepository, PersonRepository>();




            // JWT 
            services.AddScoped<IToken, TokenBusiness>();
            services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
            services.AddSingleton<IAuthCookieFactory, AuthCookieFactory>();
            services.AddScoped<Microsoft.AspNetCore.Identity.IPasswordHasher<Entity.Domain.Models.Implements.SecurityAuthentication.User>, Microsoft.AspNetCore.Identity.PasswordHasher<Entity.Domain.Models.Implements.SecurityAuthentication.User>>();

            services.AddMemoryCache();


            //Validaciones

            services.AddTransient<ExceptionMiddleware>();


            // Handlers (elige Singleton/Scoped; Singleton suele bastar)
            services.AddSingleton<IExceptionHandler, ValidationExceptionHandler>();
            services.AddSingleton<IExceptionHandler, BusinessExceptionHandler>();
            services.AddSingleton<IExceptionHandler, EntityNotFoundExceptionHandler>();
            services.AddSingleton<IExceptionHandler, ForbiddenExceptionHandler>(); 
            services.AddSingleton<IExceptionHandler, UnauthorizedAccessHandler>();
            services.AddSingleton<IExceptionHandler, DbConcurrencyExceptionHandler>();
            services.AddSingleton<IExceptionHandler, DbUpdateExceptionHandler>();     
            services.AddSingleton<IExceptionHandler, HttpRequestExceptionHandler>();  
            services.AddSingleton<IExceptionHandler, ExternalServiceExceptionHandler>();
            services.AddSingleton<IExceptionHandler, DefaultExceptionHandler>();      

            return services;
        }

    }
}