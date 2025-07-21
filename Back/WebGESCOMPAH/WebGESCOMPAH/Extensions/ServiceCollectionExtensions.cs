using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace WebGESCOMPAH.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddValidations(this IServiceCollection services)
        {
            // Registrar todos los IValidator<T> desde cualquier ensamblado
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
