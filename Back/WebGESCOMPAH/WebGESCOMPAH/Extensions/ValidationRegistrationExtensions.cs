using FluentValidation;

namespace WebGESCOMPAH.Extensions
{
    public static class ValidationRegistrationExtensions
    {
        public static IServiceCollection AddValidatorsFromAssemblyContaining<T>(this IServiceCollection services)
        {
            services.Scan(scan => scan
                .FromAssemblyOf<T>()
                .AddClasses(classes => classes.AssignableTo(typeof(IValidator<>)))
                .AsImplementedInterfaces()
                .WithScopedLifetime());

            return services;
        }
    }
}