using FluentValidation;

namespace Entity.DTOs.Validations.Persons
{
    public abstract class BasePersonValidator<T> : AbstractValidator<T>
    {
        protected BasePersonValidator()
        {
            RuleFor(x => (string?)GetPropertyValue(x, "FirstName"))
                .NotEmpty().WithMessage("El nombre es obligatorio.")
                .MaximumLength(50)
                .Matches(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s'-]+$").WithMessage("El nombre contiene caracteres inválidos.");

            RuleFor(x => (string?)GetPropertyValue(x, "LastName"))
                .NotEmpty().WithMessage("El apellido es obligatorio.")
                .MaximumLength(50)
                .Matches(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s'-]+$").WithMessage("El apellido contiene caracteres inválidos.");

            RuleFor(x => (string?)GetPropertyValue(x, "Document"))
                .NotEmpty().WithMessage("La identificación es obligatoria.")
                .MaximumLength(20)
                .Matches(@"^\d{6,20}$").WithMessage("El documento debe contener solo números entre 6 y 20 dígitos.");

            RuleFor(x => (string?)GetPropertyValue(x, "Phone"))
                .NotEmpty().WithMessage("El número de teléfono es obligatorio.")
                .Matches(@"^\+?\d{7,15}$").WithMessage("El número de teléfono debe tener entre 7 y 15 dígitos.");

            RuleFor(x => (string?)GetPropertyValue(x, "Address"))
                .NotEmpty().WithMessage("La dirección es obligatoria.")
                .MaximumLength(100)
                .Matches(@"^[\w\s\.\-#°]+$").WithMessage("La dirección contiene caracteres inválidos.");

            RuleFor(x => (int)GetPropertyValue(x, "CityId"))
                .GreaterThan(0).WithMessage("Debe seleccionar una ciudad válida.");
        }

        private object GetPropertyValue(object instance, string propertyName)
        {
            var property = instance?.GetType().GetProperty(propertyName);
            return property?.GetValue(instance) ?? throw new InvalidOperationException($"Propiedad '{propertyName}' no encontrada.");
        }
    }

}
