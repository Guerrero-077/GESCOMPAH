using Entity.DTOs.Implements.Persons.Person;
using FluentValidation;

namespace Entity.DTOs.Validations.Persons
{
    public class PersonDtoValidator : BasePersonValidator<PersonDto>
    {
        public PersonDtoValidator()
        {
            RuleFor(x => (string?)Get(x, "Document"))
                  .NotEmpty().WithMessage("La identificación es obligatoria.")
                  .MaximumLength(20)
                  .Matches(@"^\d{6,20}$").WithMessage("El documento debe contener solo números entre 6 y 20 dígitos.");
        }
    }

}
