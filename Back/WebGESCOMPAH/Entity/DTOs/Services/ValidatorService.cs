using Entity.DTOs.Interfaces;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Entity.DTOs.Services
{
    public class ValidatorService : IValidatorService
    {
        private readonly IServiceProvider _provider;

        public ValidatorService(IServiceProvider provider)
        {
            _provider = provider;
        }

        public async Task ValidateAsync<T>(T instance, CancellationToken ct = default)
        {
            var validator = _provider.GetService<IValidator<T>>();
            if (validator is null)
                throw new InvalidOperationException($"Validator not found for type {typeof(T).Name}");

            var result = await validator.ValidateAsync(instance, ct);
            if (!result.IsValid)
            {
                // ❗ Aquí pasamos directamente los errores
                throw new ValidationException(result.Errors);
            }
        }
    }
}
