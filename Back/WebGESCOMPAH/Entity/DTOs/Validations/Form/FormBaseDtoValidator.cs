using FluentValidation;
using Entity.DTOs.Implements.AdministrationSystem.Form;

namespace Entity.DTOs.Validations.Form
{
    public abstract class FormBaseDtoValidator<T> : AbstractValidator<T> where T : FormBaseDto
    {
        public FormBaseDtoValidator()
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
            RuleFor(x => x.Description).NotEmpty().MaximumLength(500);
            RuleFor(x => x.Route).MaximumLength(200).When(x => x.Route != null);
        }
    }
}
