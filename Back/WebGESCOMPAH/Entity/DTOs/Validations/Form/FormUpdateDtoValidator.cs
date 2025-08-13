using FluentValidation;
using Entity.DTOs.Implements.AdministrationSystem.Form;

namespace Entity.DTOs.Validations.Form
{
    public class FormUpdateDtoValidator : FormBaseDtoValidator<FormUpdateDto>
    {
        public FormUpdateDtoValidator()
        {
            RuleFor(x => x.Id).GreaterThan(0);
        }
    }
}
