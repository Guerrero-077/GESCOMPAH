using FluentValidation;
using Entity.DTOs.Implements.AdministrationSystem.Module;

namespace Entity.DTOs.Validations.AdministrationSystem.Module
{
    public class ModuleUpdateDtoValidator : ModuleBaseDtoValidator<ModuleUpdateDto>
    {
        public ModuleUpdateDtoValidator()
        {
            RuleFor(x => x.Id).GreaterThan(0);
        }
    }
}
