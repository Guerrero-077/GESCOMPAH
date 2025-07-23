using Business.CustomJWT;
using Business.Interfaces;
using Business.Interfaces.Implements;
using Business.Mapping;
using Business.Services;
using Common.Custom;
using Data.Interfaz.DataBasic;
using Data.Interfaz.IDataImplemenent;
using Data.Interfaz.Security;
using Data.Repository;
using Data.Services.Location;
using Data.Services.SecurityAuthentication;
using Mapster;
using Utilities.Messaging.Implements;
using Utilities.Messaging.Interfaces;

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
            services.AddScoped<IPersonService, PersonService>();
            services.AddScoped<IUserService, UserService>();


            //Mapping
            services.AddMapster();
            MapsterConfig.Register();

            //services

            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IRolUserRepository, RolUserRepository>();

            services.AddScoped<ICityRepository, CityRepository>();
            services.AddScoped<ICityService, CityService>();

            services.AddScoped<IDepartmentService, DepartmentService>();




            services.AddScoped(typeof(IDataGeneric<>), typeof(DataGeneric<>));

            // JWT 
            services.AddScoped<IToken, TokenBusiness>();





            return services;
        }

    }
}