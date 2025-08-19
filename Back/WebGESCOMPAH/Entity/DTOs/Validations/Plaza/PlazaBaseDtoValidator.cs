using FluentValidation;
using Entity.DTOs.Implements.Business.Plaza;

namespace Entity.DTOs.Validations.Plaza
{
    public abstract class PlazaBaseDtoValidator<T> : AbstractValidator<T> where T : IPlazaDto
    {
        public PlazaBaseDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("El nombre es obligatorio.")
                .MaximumLength(200);

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("La descripción es obligatoria.")
                .MaximumLength(1000);

            RuleFor(x => x.Location)
                .NotEmpty().WithMessage("La ubicación es obligatoria.")
                .MaximumLength(200);

            //RuleFor(x => x.Capacity)
            //    .GreaterThanOrEqualTo(0).WithMessage("La capacidad no puede ser negativa.");
        }
    }
}
