using System.Text.RegularExpressions;
using Entity.DTOs.Implements.Business.Plaza;
using FluentValidation;

namespace Entity.DTOs.Validations.Plaza
{
    public abstract class PlazaBaseDtoValidator<T> : AbstractValidator<T> where T : IPlazaDto
    {
        private const int NameMaxLength = 200;
        private const int DescriptionMaxLength = 1000;
        private const int LocationMaxLength = 200;
        private static readonly Regex AllowedLocationRegex = new(@"^[\p{L}\p{M}\d\s#\-,.]+$", RegexOptions.Compiled);

        protected PlazaBaseDtoValidator()
        {
            RuleFor(x => x.Name)
                .Cascade(CascadeMode.Stop)
                .Must(NotBlank)
                    .WithMessage("El nombre es obligatorio.")
                .Must(value => value!.Trim().Length <= NameMaxLength)
                    .WithMessage($"El nombre no puede superar {NameMaxLength} caracteres.");

            RuleFor(x => x.Description)
                .Cascade(CascadeMode.Stop)
                .Must(NotBlank)
                    .WithMessage("La descripcion es obligatoria.")
                .Must(value => value!.Trim().Length <= DescriptionMaxLength)
                    .WithMessage($"La descripcion no puede superar {DescriptionMaxLength} caracteres.");

            RuleFor(x => x.Location)
                .Cascade(CascadeMode.Stop)
                .Must(NotBlank)
                    .WithMessage("La ubicacion es obligatoria.")
                .Must(value => value!.Trim().Length <= LocationMaxLength)
                    .WithMessage($"La ubicacion no puede superar {LocationMaxLength} caracteres.")
                .Must(value => AllowedLocationRegex.IsMatch(value!.Trim()))
                    .WithMessage("La ubicacion contiene caracteres no permitidos.");
        }

        private static bool NotBlank(string? value) => !string.IsNullOrWhiteSpace(value);
    }
}
