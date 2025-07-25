using Entity.DTOs.Implements.Persons.Peron;
using FluentValidation;

namespace Entity.DTOs.Validations.Persons
{
    public class PersonDtoValidator : AbstractValidator<PersonDto>
    {
        public PersonDtoValidator()
        {
            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("El nombre es obligatorio.")
                .MaximumLength(50);

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("El apellido es obligatorio.")
                .MaximumLength(50);

            RuleFor(x => x.Document)
                .NotEmpty().WithMessage("La identificación es obligatoria.")
                .MaximumLength(20);

            RuleFor(x => x.Phone)
                .NotEmpty().WithMessage("El número de teléfono es obligatorio.")
                .Matches(@"^\+?\d{7,15}$").WithMessage("Formato de teléfono inválido.");

            RuleFor(x => x.Address)
                .NotEmpty().WithMessage("La dirección es obligatoria.")
                .MaximumLength(100);

            RuleFor(x => x.CityId)
                .GreaterThan(0).WithMessage("Debe seleccionar una ciudad válida.");
        }
    }
}
