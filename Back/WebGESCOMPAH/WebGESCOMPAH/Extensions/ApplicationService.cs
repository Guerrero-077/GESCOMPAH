﻿using Business.CQRS.Behaviors;
using Business.CQRS.Business.Establishments.Create;
using Business.CustomJWT;
using Business.Interfaces;
using Business.Interfaces.Implements.AdministrationSystem;
using Business.Interfaces.Implements.Business;
using Business.Interfaces.Implements.Location;
using Business.Interfaces.Implements.Persons;
using Business.Interfaces.Implements.SecrutityAuthentication;
using Business.Interfaces.Implements.Utilities;
using Business.Mapping;
using Business.Services.Business;
using Business.Services.Location;
using Business.Services.SecrutityAuthentication;
using Business.Services.Utilities;
using Data.Interfaz.DataBasic;
using Data.Interfaz.IDataImplemenent;
using Data.Interfaz.Security;
using Data.Repository;
using Data.Services.Location;
using Data.Services.SecurityAuthentication;
using Mapster;
using MediatR;
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

            //Services
            services.AddScoped<IPersonService, PersonService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IRolService, RolService>();
            services.AddScoped<IPlazaService, PlazasService>();
            services.AddScoped<IEstablishmentService, EstablishmentService>();
            services.AddScoped<IImagesService, ImageService>(); 

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

            //Validation

            services.AddValidatorsFromAssemblyContaining<CreateEstablishmentCommandValidator>();

            // Registro global del pipeline de validación
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

            return services;
        }

    }
}