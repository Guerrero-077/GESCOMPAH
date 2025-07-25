using Entity.DTOs.Implements.Business.Establishment;
using FluentValidation;

namespace Entity.DTOs.Validations
{
    public class EstablishmentCreateDtoValidator : AbstractValidator<EstablishmentCreateDto>
    {
        public EstablishmentCreateDtoValidator()
        {
            RuleFor(x => x)
                .Cascade(CascadeMode.Stop)
                .Custom((dto, context) =>
                {
                    if (string.IsNullOrWhiteSpace(dto.Name))
                        context.AddFailure(nameof(dto.Name), "El nombre es obligatorio.");

                    if (string.IsNullOrWhiteSpace(dto.Description))
                        context.AddFailure(nameof(dto.Description), "La descripción es obligatoria.");

                    if (dto.AreaM2 <= 0)
                        context.AddFailure(nameof(dto.AreaM2), "El área debe ser mayor a 0.");

                    if (dto.RentValueBase < 0)
                        context.AddFailure(nameof(dto.RentValueBase), "El valor de renta no puede ser negativo.");

                    if (dto.Files != null)
                    {
                        foreach (var file in dto.Files)
                        {
                            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
                            var allowed = new[] { ".jpg", ".jpeg", ".png", ".pdf" };

                            if (!allowed.Contains(extension))
                                context.AddFailure(nameof(dto.Files), $"El archivo '{file.FileName}' tiene una extensión inválida.");

                            if (file.Length > 5 * 1024 * 1024)
                                context.AddFailure(nameof(dto.Files), $"El archivo '{file.FileName}' supera el tamaño máximo permitido (5 MB).");
                        }
                    }
                });
        }
    }
}