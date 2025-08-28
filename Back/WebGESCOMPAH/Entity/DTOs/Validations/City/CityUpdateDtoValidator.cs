using Entity.DTOs.Implements.Location.City;
using Entity.DTOs.Implements.Location.Department;
using FluentValidation;
using System.Text.RegularExpressions;

namespace Entity.DTOs.Validations.City
{
    public class DepartmentUpdateDtoValidator : AbstractValidator<DepartmentUpdateDto>
    {
        public DepartmentUpdateDtoValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("El identificador del departamento es inv�lido.");

            RuleFor(x => x.Name)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El nombre es obligatorio.")
                .Must(v => !string.IsNullOrWhiteSpace(v))
                    .WithMessage("El nombre no puede estar en blanco.")
                .Must(v => v == v.Trim())
                    .WithMessage("El nombre no debe iniciar ni terminar con espacios.")
                .Length(2, 80)
                    .WithMessage("El nombre debe tener entre 2 y 80 caracteres.")
                .Matches(@"^[A-Za-z��������������'\.\- ]+$")
                    .WithMessage("El nombre solo puede contener letras, espacios, ap�strofes, puntos y guiones.")
                .Must(v => !Regex.IsMatch(v, @"\s{2,}"))
                    .WithMessage("El nombre no debe contener espacios consecutivos.");
        }
    }
}
