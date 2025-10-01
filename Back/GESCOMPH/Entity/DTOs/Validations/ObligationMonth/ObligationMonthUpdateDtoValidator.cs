using Entity.DTOs.Implements.Business.ObligationMonth;
using FluentValidation;

namespace Entity.DTOs.Validations.ObligationMonth
{
    public class ObligationMonthUpdateDtoValidator : ObligationMonthBaseDtoValidator<ObligationMonthUpdateDto>
    {
        public ObligationMonthUpdateDtoValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0)
                    .WithMessage("El identificador es obligatorio.");
        }
    }
}
