using Entity.DTOs.Implements.AdministrationSystem.FormModule;
using FluentValidation;

namespace Entity.DTOs.Validations.AdministrationSystem.FormModule
{
    public class FormModuleUpdateDtoValidator : AbstractValidator<FormModuleUpdateDto>
    {
        public FormModuleUpdateDtoValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("El identificador es obligatorio.");

            RuleFor(x => x.FormId)
                .GreaterThan(0).WithMessage("Debes seleccionar un formulario válido.");

            RuleFor(x => x.ModuleId)
                .GreaterThan(0).WithMessage("Debes seleccionar un módulo válido.");
        }
    }
}
