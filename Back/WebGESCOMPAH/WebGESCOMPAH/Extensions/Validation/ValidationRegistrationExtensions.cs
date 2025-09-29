using FluentValidation;

namespace WebGESCOMPAH.Extensions.Validation
{
    public static class ValidationRegistrationExtensions
    {
        public static IServiceCollection AddValidatorsFromAssemblyContaining<T>(this IServiceCollection services)
        {
            services.Scan(scan => scan
                .FromAssemblyOf<T>()
                .AddClasses(c => c.AssignableTo(typeof(IValidator<>)))
                .AsImplementedInterfaces()
                .WithScopedLifetime());

            return services;
        }
    }
}

