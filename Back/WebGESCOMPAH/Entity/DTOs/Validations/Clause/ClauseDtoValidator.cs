using Entity.DTOs.Implements.Business.Clause;
using FluentValidation;

namespace Entity.DTOs.Validations.Clause
{
    public class ClauseDtoValidator : AbstractValidator<ClauseDto>
    {
        public ClauseDtoValidator()
        {
            RuleFor(x => x.Name)
                .Cascade(CascadeMode.Stop)
                .Must(value => !string.IsNullOrWhiteSpace(value))
                    .WithMessage("El nombre es obligatorio.")
                .MaximumLength(120)
                    .WithMessage("El nombre no puede superar 120 caracteres.");

            RuleFor(x => x.Description)
                .Cascade(CascadeMode.Stop)
                .Must(value => !string.IsNullOrWhiteSpace(value))
                    .WithMessage("La descripcion es obligatoria.")
                .MaximumLength(4000)
                    .WithMessage("La descripcion no puede superar 4000 caracteres.");
        }
    }
}
