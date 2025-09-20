using System;
using System.Text.RegularExpressions;
using Entity.DTOs.Implements.Business.Calculation;
using FluentValidation;

namespace Entity.DTOs.Validations.Calculation
{
    public class CalculationCreateDtoValidator : AbstractValidator<CalculationCreateDto>
    {
        private static readonly Regex TokenRegex = new(@"^[A-Za-z0-9._-]+$", RegexOptions.Compiled);

        public CalculationCreateDtoValidator()
        {
            RuleFor(x => x.Type)
                .Cascade(CascadeMode.Stop)
                .Must(value => !string.IsNullOrWhiteSpace(value))
                    .WithMessage("El tipo es obligatorio.")
                .Must(value => value == null || value.Trim().Length <= 50)
                    .WithMessage("El tipo no puede superar 50 caracteres.")
                .Must(value => value == null || TokenRegex.IsMatch(value.Trim()))
                    .WithMessage("El tipo contiene caracteres no validos.");

            RuleFor(x => x.Key)
                .Cascade(CascadeMode.Stop)
                .Must(value => !string.IsNullOrWhiteSpace(value))
                    .WithMessage("La clave es obligatoria.")
                .Must(value => value == null || value.Trim().Length <= 100)
                    .WithMessage("La clave no puede superar 100 caracteres.")
                .Must(value => value == null || TokenRegex.IsMatch(value.Trim()))
                    .WithMessage("La clave contiene caracteres no validos.");

            RuleFor(x => x.Formula)
                .Cascade(CascadeMode.Stop)
                .Must(value => !string.IsNullOrWhiteSpace(value))
                    .WithMessage("La formula es obligatoria.")
                .Must(value => value == null || value.Trim().Length <= 4000)
                    .WithMessage("La formula no puede superar 4000 caracteres.");

            RuleFor(x => x.Description)
                .Cascade(CascadeMode.Stop)
                .Must(value => string.IsNullOrWhiteSpace(value) || value.Trim().Length <= 500)
                    .WithMessage("La descripcion no puede superar 500 caracteres.");

            RuleFor(x => x.EffectiveFrom)
                .NotEqual(default(DateTime))
                    .WithMessage("La fecha de inicio de vigencia es obligatoria.");

            RuleFor(x => x.EffectiveTo)
                .Must((dto, effectiveTo) => effectiveTo is null || effectiveTo.Value.Date >= dto.EffectiveFrom.Date)
                    .WithMessage("La fecha final de vigencia no puede ser anterior a la fecha inicial.");
        }
    }
}
