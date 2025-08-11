using FluentValidation;
using Entity.DTOs.Implements.Location.City;

namespace Entity.DTOs.Validations.City
{
    public abstract class CityBaseDtoValidator<T> : AbstractValidator<T> where T : CityBaseDto
    {
        public CityBaseDtoValidator()
        {
            RuleFor(x => x.Id).GreaterThan(0);
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("El nombre es obligatorio.")
                .MaximumLength(200);

            RuleFor(x => x.DepartmentId)
                .GreaterThan(0).WithMessage("Debe seleccionar un departamento válido.");
        }
    }
}
