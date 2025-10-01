using FluentValidation;
using Entity.DTOs.Implements.Utilities.Images;

namespace Entity.DTOs.Validations.Images
{
    public class ImageUpdateDtoValidator : AbstractValidator<ImageUpdateDto>
    {
        public ImageUpdateDtoValidator()
        {
            RuleFor(x => x.Id).GreaterThan(0);
            RuleFor(x => x.File)
                .NotNull().WithMessage("El archivo es obligatorio.")
                .Must(f => f.Length > 0).WithMessage("El archivo está vacío.")
                .Must(f =>
                {
                    var ext = System.IO.Path.GetExtension(f.FileName).ToLowerInvariant();
                    var allowed = new[] { ".jpg", ".jpeg", ".png", ".webp" };
                    return allowed.Contains(ext);
                }).WithMessage("Extensión de archivo inválida. Permitido: .jpg, .jpeg, .png, .webp");
        }
    }
}
