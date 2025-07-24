using FluentValidation;

namespace Business.CQRS.Business.Establishments.Create
{
    public class CreateEstablishmentCommandValidator : AbstractValidator<CreateEstablishmentCommand>
    {
        public CreateEstablishmentCommandValidator()
        {
            RuleFor(x => x.Dto.Name)
                .NotEmpty().WithMessage("El nombre es obligatorio.")
                .MaximumLength(100);

            RuleFor(x => x.Dto.Description)
                .NotEmpty().WithMessage("La descripción es obligatoria.");

            RuleFor(x => x.Dto.AreaM2)
                .GreaterThan(0).WithMessage("El área debe ser mayor a cero.");

            RuleFor(x => x.Dto.RentValueBase)
                .GreaterThanOrEqualTo(0).WithMessage("El valor de renta no puede ser negativo.");

            RuleFor(x => x.Dto.Files)
                .Must(files => files == null || files.Count <= 5)
                .WithMessage("Solo se permiten hasta 5 imágenes.");

            RuleForEach(x => x.Dto.Files)
                .Must(file => file.ContentType == "image/jpeg" || file.ContentType == "image/png")
                .WithMessage("Solo se permiten imágenes en formato JPEG o PNG.");
        }
    }
}
