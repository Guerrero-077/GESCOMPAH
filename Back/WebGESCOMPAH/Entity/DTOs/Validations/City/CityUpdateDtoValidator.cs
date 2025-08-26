using FluentValidation;
using Entity.DTOs.Implements.Location.City;

namespace Entity.DTOs.Validations.City
{
    public class CityUpdateDtoValidator : CityDtoBaseValidator<CityUpdateDto>
    {
        public CityUpdateDtoValidator()
        {
            // Asumiendo que BaseDto expone la propiedad Id
            RuleFor(x => x.Id)
                .GreaterThan(0)
                .WithMessage("El identificador de la ciudad es inválido.");

            RuleFor(x => x.DepartmentId)
                .GreaterThan(0)
                .WithMessage("El departamento es obligatorio.");
        }
    }
}
