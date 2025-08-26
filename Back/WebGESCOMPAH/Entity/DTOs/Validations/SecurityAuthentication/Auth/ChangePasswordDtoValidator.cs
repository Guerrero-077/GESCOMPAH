using Entity.DTOs.Implements.SecurityAuthentication.User;
using FluentValidation;

namespace Entity.DTOs.Validations.SecurityAuthentication.Auth
{
    public class ChangePasswordDtoRegexValidator : AbstractValidator<ChangePasswordDto>
    {
        // Debe tener: min 8, max 100, 1 mayúscula, 1 minúscula, 1 dígito, 1 símbolo ([\W_]).
        // Permite espacios porque tu front usa [\W_]. Si quieres prohibirlos, añade (?!.*\s)
        private const string StrongPasswordPattern =
            @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).{8,100}$";

        public ChangePasswordDtoRegexValidator()
        {
            RuleFor(x => x.UserId)
                .GreaterThan(0).WithMessage("El usuario es inválido.");

            RuleFor(x => x.CurrentPassword)
                .NotEmpty().WithMessage("La contraseña actual es obligatoria.");

            RuleFor(x => x.NewPassword)
                .NotEmpty().WithMessage("La nueva contraseña es obligatoria.")
                .Matches(StrongPasswordPattern)
                    .WithMessage("La nueva contraseña debe tener entre 8 y 100 caracteres, e incluir al menos una mayúscula, una minúscula, un dígito y un símbolo.")
                .Must((dto, newPass) => !string.Equals(dto.CurrentPassword, newPass))
                    .WithMessage("La nueva contraseña no puede ser igual a la actual.");
        }
    }

}
