using FluentValidation;
using Entity.DTOs.Implements.Location.Department;

namespace Entity.DTOs.Validations.Department
{
    // Entity.DTOs.Validations/Location/Department/DepartmentCreateDtoValidator.cs
    using System.Text.RegularExpressions;
    using FluentValidation;
    //using Entity.DTOs.Implements.Location.City;
    //using Entity.DTOs.Implements.Location.Department;

    namespace Entity.DTOs.Validations.Location.Department
    {
        public class DepartmentCreateDtoValidator : AbstractValidator<DepartmentCreateDto>
        {
            public DepartmentCreateDtoValidator()
            {
                RuleFor(x => x.Name)
                    .Cascade(CascadeMode.Stop)
                    .NotEmpty().WithMessage("El nombre es obligatorio.")
                    .Must(v => !string.IsNullOrWhiteSpace(v))
                        .WithMessage("El nombre no puede estar en blanco.")
                    .Must(v => v == v.Trim())
                        .WithMessage("El nombre no debe iniciar ni terminar con espacios.")
                    .Length(2, 80)
                        .WithMessage("El nombre debe tener entre 2 y 80 caracteres.")
                    .Matches(@"^[A-Za-zÁÉÍÓÚÜÑáéíóúüñ'\.\- ]+$")
                        .WithMessage("El nombre solo puede contener letras, espacios, apóstrofes, puntos y guiones.")
                    .Must(v => !Regex.IsMatch(v, @"\s{2,}"))
                        .WithMessage("El nombre no debe contener espacios consecutivos.");

                RuleFor(x => x.Cities)
                    .NotNull().WithMessage("La lista de ciudades no puede ser nula.");

                RuleForEach(x => x.Cities).ChildRules(city =>
                {
                    city.RuleFor(c => c.Name)
                        .Cascade(CascadeMode.Stop)
                        .NotEmpty().WithMessage("El nombre de la ciudad es obligatorio.")
                        .Must(v => !string.IsNullOrWhiteSpace(v))
                            .WithMessage("El nombre de la ciudad no puede estar en blanco.")
                        .Must(v => v == v.Trim())
                            .WithMessage("El nombre de la ciudad no debe iniciar ni terminar con espacios.")
                        .Length(2, 80)
                            .WithMessage("El nombre de la ciudad debe tener entre 2 y 80 caracteres.")
                        .Matches(@"^[A-Za-zÁÉÍÓÚÜÑáéíóúüñ'\.\- ]+$")
                            .WithMessage("El nombre de la ciudad solo puede contener letras, espacios, apóstrofes, puntos y guiones.")
                        .Must(v => !Regex.IsMatch(v, @"\s{2,}"))
                            .WithMessage("El nombre de la ciudad no debe contener espacios consecutivos.");

                    city.RuleFor(c => c.DepartmentId)
                        .GreaterThan(0)
                        .WithMessage("El departamento de la ciudad es obligatorio.");
                });
            }
        }
    }

}
