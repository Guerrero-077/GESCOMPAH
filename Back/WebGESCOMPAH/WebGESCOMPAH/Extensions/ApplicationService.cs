using Business.CustomJWT;
using Business.Interfaces;
using Business.Interfaces.Implements;
using Business.Mapping;
using Business.Services;
using Data.Interfaz.DataBasic;
using Data.Interfaz.IDataImplemenent;
using Data.Repository;
using Data.Services;
using Mapster;
using Utilities.Custom;
using Utilities.Messaging.Implements;
using Utilities.Messaging.Interfaces;

namespace WebGESCOMPAH.Extensions
{
    public static class ApplicationService
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {


            //Email
            services.AddTransient<ISendCode, EmailService>();

            //Auth
            services.AddScoped<IPasswordResetCodeRepository, PasswordResetCodeRepository>();
            services.AddScoped<EncriptePassword>();
            services.AddScoped<IAuthService, AuthService>();


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