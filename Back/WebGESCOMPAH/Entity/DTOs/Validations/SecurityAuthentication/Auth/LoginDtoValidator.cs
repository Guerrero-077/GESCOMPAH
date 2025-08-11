using Entity.DTOs.Implements.SecurityAuthentication.Auth;
using FluentValidation;

namespace Entity.DTOs.Validations.SecurityAuthentication.Auth
{
    
    public class LoginDtoValidator : AbstractValidator<LoginDto>
    {
        public LoginDtoValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("El correo electrónico es obligatorio.")
                .EmailAddress().WithMessage("Formato de correo electrónico inválido.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("La contraseña es obligatoria.");
        }
    }
}
