using Entity.DTOs.Implements.Persons.Peron;
using FluentValidation;

namespace Entity.DTOs.Validations.Persons
{
    public class PersonUpdateDtoValidator : BasePersonValidator<PersonUpdateDto>
    {
        public PersonUpdateDtoValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("El ID es obligatorio para actualizar.");
        }
    }
}
