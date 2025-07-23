using Entity.DTOs.Implements.SecurityAuthentication.Auth;
using FluentValidation;

namespace Entity.DTOs.Validations
{
    namespace Entity.DTOs.Validators.SecurityAuthentication
    {
        public class RegisterDtoValidator : AbstractValidator<RegisterDto>
        {
            public RegisterDtoValidator()
            {
                RuleFor(x => x.Email)
                    .NotEmpty().WithMessage("El correo electrónico es obligatorio.")
                    .EmailAddress().WithMessage("Formato de correo electrónico inválido.");

                RuleFor(x => x.Password)
                    .NotEmpty().WithMessage("La contraseña es obligatoria.")
                    .MinimumLength(6).WithMessage("La contraseña debe tener al menos 6 caracteres.")
                    .MaximumLength(100).WithMessage("La contraseña no puede exceder los 100 caracteres.");

                RuleFor(x => x.FirstName)
                    .NotEmpty().WithMessage("El nombre es obligatorio.")
                    .MaximumLength(50).WithMessage("El nombre no puede exceder los 50 caracteres.");

                RuleFor(x => x.LastName)
                    .NotEmpty().WithMessage("El apellido es obligatorio.")
                    .MaximumLength(50).WithMessage("El apellido no puede exceder los 50 caracteres.");

                RuleFor(x => x.Document)
                    .NotEmpty().WithMessage("La identificación es obligatoria.")
                    .MaximumLength(20).WithMessage("La identificación no puede exceder los 20 caracteres.");

                RuleFor(x => x.Phone)
                    .NotEmpty().WithMessage("El número de teléfono es obligatorio.")
                    .Matches(@"^\+?\d{7,15}$").WithMessage("El número de teléfono no tiene un formato válido.");

                RuleFor(x => x.Address)
                    .NotEmpty().WithMessage("La dirección es obligatoria.")
                    .MaximumLength(100).WithMessage("La dirección no puede exceder los 100 caracteres.");

                RuleFor(x => x.CityId)
                    .GreaterThan(0).WithMessage("Debe seleccionar una ciudad válida.");
            }
        }
    }
}