using System;
using System.Text.RegularExpressions;
using Entity.DTOs.Implements.AdministrationSystem.SystemParameter;
using FluentValidation;

namespace Entity.DTOs.Validations.AdministrationSystem.SystemParameter
{
    public abstract class SystemParameterBaseValidator<T> : AbstractValidator<T> where T : ISystemParameterDto
    {
        private static readonly Regex KeyRegex = new Regex("^[A-Z0-9_.-]+$", RegexOptions.Compiled);

        protected SystemParameterBaseValidator()
        {
            RuleFor(x => x.Key)
                .Cascade(CascadeMode.Stop)
                .Must(value => !string.IsNullOrWhiteSpace(value)).WithMessage("La clave es obligatoria.")
                .MaximumLength(100).WithMessage("La clave no puede superar 100 caracteres.")
                .Matches(KeyRegex).WithMessage("La clave solo puede incluir letras mayúsculas, números, puntos, guiones y guiones bajos.");

            RuleFor(x => x.Value)
                .Cascade(CascadeMode.Stop)
                .Must(value => !string.IsNullOrWhiteSpace(value)).WithMessage("El valor es obligatorio.")
                .MaximumLength(200).WithMessage("El valor no puede superar 200 caracteres.");

            RuleFor(x => x.EffectiveFrom)
                .NotEqual(default(DateTime)).WithMessage("La fecha de vigencia inicial es obligatoria.");

            RuleFor(x => x)
                .Custom((dto, context) =>
                {
                    if (dto.EffectiveTo.HasValue && dto.EffectiveTo.Value < dto.EffectiveFrom)
                        context.AddFailure(nameof(dto.EffectiveTo), "La fecha de vigencia final debe ser igual o posterior a la inicial.");
                });
        }
    }
}
