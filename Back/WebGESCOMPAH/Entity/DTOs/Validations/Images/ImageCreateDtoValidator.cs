using FluentValidation;
using Entity.DTOs.Implements.Utilities.Images;

namespace Entity.DTOs.Validations.Images
{
    public class ImageCreateDtoValidator : AbstractValidator<ImageCreateDto>
    {
        public ImageCreateDtoValidator()
        {
            RuleFor(x => x.FileName).NotEmpty().MaximumLength(255);
            RuleFor(x => x.FilePath).NotEmpty();
            RuleFor(x => x.PublicId).NotEmpty();
            RuleFor(x => x.EstablishmentId).GreaterThan(0);
        }
    }
}
