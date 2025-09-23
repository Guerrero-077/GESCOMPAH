using Entity.DTOs.Implements.AdministrationSystem.SystemParameter;
using FluentValidation;

namespace Entity.DTOs.Validations.AdministrationSystem.SystemParameter
{
    public class SystemParameterUpdateDtoValidator : SystemParameterBaseValidator<SystemParameterUpdateDto>
    {
        public SystemParameterUpdateDtoValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("El identificador es obligatorio.");
        }
    }
}
