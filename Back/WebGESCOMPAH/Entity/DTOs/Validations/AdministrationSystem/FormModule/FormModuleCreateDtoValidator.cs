using Entity.DTOs.Implements.AdministrationSystem.FormModule;
using FluentValidation;

namespace Entity.DTOs.Validations.AdministrationSystem.FormModule
{
    public class FormModuleCreateDtoValidator : AbstractValidator<FormModuleCreateDto>
    {
        public FormModuleCreateDtoValidator()
        {
            RuleFor(x => x.FormId)
                .GreaterThan(0).WithMessage("Debes seleccionar un formulario válido.");

            RuleFor(x => x.ModuleId)
                .GreaterThan(0).WithMessage("Debes seleccionar un módulo válido.");
        }
    }
}
