using System.Reflection;
using FluentValidation;

namespace Entity.DTOs.Validations.Persons
{
    public abstract class BasePersonValidator<T> : AbstractValidator<T>
    {
        protected BasePersonValidator()
        {
            RuleFor(x => (string?)Get(x, "FirstName"))
              .NotEmpty().WithMessage("El nombre es obligatorio.")
              .MaximumLength(50)
              .Matches(@"^[\p{L}\p{M}\s'-]+$").WithMessage("El nombre contiene caracteres inválidos.");

            RuleFor(x => (string?)Get(x, "LastName"))
              .NotEmpty().WithMessage("El apellido es obligatorio.")
              .MaximumLength(50)
              .Matches(@"^[\p{L}\p{M}\s'-]+$").WithMessage("El apellido contiene caracteres inválidos.");

            RuleFor(x => (string?)Get(x, "Phone"))
              .NotEmpty().WithMessage("El número de teléfono es obligatorio.")
              .Matches(@"^\+?\d{7,15}$").WithMessage("El número de teléfono debe tener entre 7 y 15 dígitos.");

            RuleFor(x => (string?)Get(x, "Address"))
              .NotEmpty().WithMessage("La dirección es obligatoria.")
              .MaximumLength(100)
              .Matches(@"^[\w\s\.\-#°]+$").WithMessage("La dirección contiene caracteres inválidos.");

            // CityId como nullable para no castear null a int
            RuleFor(x => (int?)Get(x, "CityId"))
              .NotNull().WithMessage("Debe seleccionar una ciudad válida.")
              .GreaterThan(0).WithMessage("Debe seleccionar una ciudad válida.");
        }

        // Hacemos el helper accesible y tolerante: ignora may/min y no lanza
        protected static object? Get(object? instance, string propertyName)
        {
            if (instance is null) return null;
            var prop = instance.GetType().GetProperty(
              propertyName,
              BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase
            );
            return prop?.GetValue(instance);
        }
    }
}
