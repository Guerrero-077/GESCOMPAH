using Entity.DTOs.Implements.Persons.Person;
using FluentValidation;

namespace Entity.DTOs.Validations.Persons
{
    public class PersonDtoValidator : BasePersonValidator<PersonDto>
    {
        public PersonDtoValidator()
        {
            // Si deseas agregar reglas exclusivas de creación aquí
        }
    }

}
