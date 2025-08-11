using FluentValidation;
using Entity.DTOs.Implements.Location.Department;

namespace Entity.DTOs.Validations.Department
{
    public class DepartmentCreateDtoValidator : AbstractValidator<DepartmentCreateDto>
    {
        public DepartmentCreateDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("El nombre es obligatorio.")
                .MaximumLength(200);

            RuleForEach(x => x.Cities)
                .SetValidator(new City.CityCreateDtoValidator());
        }
    }
}
