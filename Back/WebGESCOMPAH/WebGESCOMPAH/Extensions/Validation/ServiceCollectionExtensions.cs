using FluentValidation;
using System.Reflection;

namespace WebGESCOMPAH.Extensions.Validation
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddValidations(this IServiceCollection services)
        {
            // Registra todos los IValidator<T> desde el ensamblado de Entity
            var entityAssembly = AppDomain.CurrentDomain
                .GetAssemblies()
                .FirstOrDefault(a => a.GetName().Name == "Entity")
                ?? Assembly.Load("Entity");

            services.Scan(scan => scan
                .FromAssemblies(entityAssembly)
                .AddClasses(c => c.AssignableTo(typeof(IValidator<>)))
                .AsImplementedInterfaces()
                .WithScopedLifetime());


            return services;
        }
    }
}

