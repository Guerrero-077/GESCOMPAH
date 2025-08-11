using Entity.DTOs.Implements.SecurityAuthentication.Me;
using Entity.DTOs.Validations.Persons;
using FluentValidation;

namespace Entity.DTOs.Validations.SecurityAuthentication
{
    public class CreateUserValidator : AbstractValidator<UserDto>
    {
        public CreateUserValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("El correo es obligatorio.")
                .EmailAddress().WithMessage("El correo no tiene un formato válido.");

            RuleFor(x => x.Person)
                .NotNull().WithMessage("Los datos personales son obligatorios.")
                .SetValidator(new PersonDtoValidator());

            //RuleFor(x => x.Roles)
            //    .Cascade(CascadeMode.Stop)
            //    .NotNull().WithMessage("Debe especificar al menos un rol.")
            //    .Must(r => r.Any()).WithMessage("Debe incluir al menos un rol.")
            //    .Must(AllValidRoles).WithMessage("Uno o más roles no son válidos.");
        }

        //private bool AllValidRoles(List<string> roles)
        //{
        //    var allowedRoles = new[] { "Admin", "User", "Manager" }; // Puedes mover esto a configuración
        //    return roles.All(role => allowedRoles.Contains(role));
        //}
    }
}
