using Entity.DTOs.Implements.SecurityAuthentication.RolUser;
using FluentValidation;

namespace Entity.DTOs.Validations.SecurityAuthentication.RolUser
{
    public class RolUserCreateDtoValidator : AbstractValidator<RolUserCreateDto>
    {
        public RolUserCreateDtoValidator()
        {
            RuleFor(x => x.RolId)
                .GreaterThan(0)
                    .WithMessage("Debe seleccionar un rol valido.");

            RuleFor(x => x.UserId)
                .GreaterThan(0)
                    .WithMessage("Debe seleccionar un usuario valido.");
        }
    }
}
