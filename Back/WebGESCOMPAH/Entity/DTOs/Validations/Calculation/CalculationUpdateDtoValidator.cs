using Entity.DTOs.Implements.Business.Calculation;
using FluentValidation;

namespace Entity.DTOs.Validations.Calculation
{
    public class CalculationUpdateDtoValidator : AbstractValidator<CalculationUpdateDto>
    {
        public CalculationUpdateDtoValidator()
        {
            Include(new CalculationCreateDtoValidator());

            RuleFor(x => x.Id)
                .GreaterThan(0)
                    .WithMessage("El identificador es obligatorio.");
        }
    }
}
