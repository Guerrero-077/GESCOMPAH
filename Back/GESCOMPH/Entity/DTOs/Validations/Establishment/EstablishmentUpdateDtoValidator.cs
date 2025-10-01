using FluentValidation;
using Entity.DTOs.Implements.Business.EstablishmentDto;

namespace Entity.DTOs.Validations.Establishment
{
    public class EstablishmentUpdateDtoValidator : AbstractValidator<EstablishmentUpdateDto>
    {
        public EstablishmentUpdateDtoValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("El Id debe ser mayor a 0.");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("El nombre es obligatorio.")
                .MaximumLength(200).WithMessage("El nombre no puede exceder 200 caracteres.");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("La descripción es obligatoria.")
                .MaximumLength(1000).WithMessage("La descripción no puede exceder 1000 caracteres.");

            RuleFor(x => x.Address)
                .NotEmpty().WithMessage("La dirección es obligatoria.")
                .MaximumLength(200).WithMessage("La dirección no puede exceder 200 caracteres.");

            RuleFor(x => x.AreaM2)
                .GreaterThanOrEqualTo(0).WithMessage("El área debe ser mayor o igual a 0.");

            RuleFor(x => x.RentValueBase)
                .GreaterThanOrEqualTo(0).WithMessage("El valor de renta no puede ser negativo.");

            RuleFor(x => x.PlazaId)
                .GreaterThan(0).WithMessage("Debe seleccionar una plaza válida.");
        }
    }
}
