using Entity.DTOs.Implements.SecurityAuthentication;
using FluentValidation;

namespace Entity.DTOs.Validations
{
    public class CreateUserValidator : AbstractValidator<UserDto>
    {
        public CreateUserValidator()
        {
            RuleFor(x => x.Email).EmailAddress();
        }
    }

}
