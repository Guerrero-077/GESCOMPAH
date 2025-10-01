using FluentValidation;
using Entity.DTOs.Implements.AdministrationSystem.Module;

namespace Entity.DTOs.Validations.AdministrationSystem.Module
{
    public abstract class ModuleBaseDtoValidator<T> : AbstractValidator<T> where T : IModuleDto
    {
        public ModuleBaseDtoValidator()
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
            RuleFor(x => x.Description).NotEmpty().MaximumLength(500);
            RuleFor(x => x.Icon).NotEmpty().MaximumLength(100);
        }
    }
}
