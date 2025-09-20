using System.Text.RegularExpressions;
using Entity.DTOs.Implements.SecurityAuthentication.Rol;
using FluentValidation;

namespace Entity.DTOs.Validations.SecurityAuthentication.Rol
{
    public class RolCreateDtoValidator : AbstractValidator<RolCreateDto>
    {
        private const int NameMaxLength = 100;
        private const int DescriptionMaxLength = 500;
        private static readonly Regex NameRegex = new(@"^[\p{L}\p{M}\d\s._-]+$", RegexOptions.Compiled);

        public RolCreateDtoValidator()
        {
            RuleFor(x => x.Name)
                .Cascade(CascadeMode.Stop)
                .Must(NotBlank)
                    .WithMessage("El nombre es obligatorio.")
                .Must(value => value!.Trim().Length <= NameMaxLength)
                    .WithMessage($"El nombre no puede superar {NameMaxLength} caracteres.")
                .Must(value => NameRegex.IsMatch(value!.Trim()))
                    .WithMessage("El nombre contiene caracteres no permitidos.");

            RuleFor(x => x.Description)
                .Cascade(CascadeMode.Stop)
                .Must(value => value is null || value.Trim().Length <= DescriptionMaxLength)
                    .WithMessage($"La descripcion no puede superar {DescriptionMaxLength} caracteres.");
        }

        private static bool NotBlank(string? value) => !string.IsNullOrWhiteSpace(value);
    }
}
