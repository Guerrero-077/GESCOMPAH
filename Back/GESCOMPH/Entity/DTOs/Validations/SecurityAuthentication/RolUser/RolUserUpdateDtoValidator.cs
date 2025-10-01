using Entity.DTOs.Implements.SecurityAuthentication.RolUser;
using FluentValidation;

namespace Entity.DTOs.Validations.SecurityAuthentication.RolUser
{
    public class RolUserUpdateDtoValidator : AbstractValidator<RolUserUpdateDto>
    {
        public RolUserUpdateDtoValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0)
                    .WithMessage("El identificador es obligatorio.");

            RuleFor(x => x.RolId)
                .GreaterThan(0)
                    .WithMessage("Debe seleccionar un rol valido.");

            RuleFor(x => x.UserId)
                .GreaterThan(0)
                    .WithMessage("Debe seleccionar un usuario valido.");
        }
    }
}
