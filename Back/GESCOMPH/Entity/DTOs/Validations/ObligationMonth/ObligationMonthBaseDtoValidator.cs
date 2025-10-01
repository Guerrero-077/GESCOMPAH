using System;
using System.Linq;
using Entity.DTOs.Implements.Business.ObligationMonth;
using FluentValidation;

namespace Entity.DTOs.Validations.ObligationMonth
{
    public abstract class ObligationMonthBaseDtoValidator<T> : AbstractValidator<T>
        where T : IObligationMonthDto
    {
        private static readonly string[] AllowedStatuses = ["PENDING", "PAID", "CANCELLED"];
        private static readonly string AllowedStatusesMessage = $"El estado debe ser uno de: {string.Join(", ", AllowedStatuses)}.";
        private const decimal MaxMoney = 1_000_000_000m;
        private const decimal MaxUvtQty = 100_000m;
        private const decimal MaxVatRate = 1m;
        private const int MaxDaysLate = 10 * 365;
        private const decimal DecimalTolerance = 0.01m;

        protected ObligationMonthBaseDtoValidator()
        {
            RuleFor(x => x.ContractId)
                .GreaterThan(0)
                    .WithMessage("El contrato es obligatorio.");

            RuleFor(x => x.Year)
                .InclusiveBetween(2000, 2100)
                    .WithMessage("El año debe estar entre 2000 y 2100.");

            RuleFor(x => x.Month)
                .InclusiveBetween(1, 12)
                    .WithMessage("El mes debe estar entre 1 y 12.");

            RuleFor(x => x.DueDate)
                .Must(date => date != default)
                    .WithMessage("La fecha de vencimiento es obligatoria.");

            RuleFor(x => x.UvtQtyApplied)
                .Cascade(CascadeMode.Stop)
                .Must(value => value >= 0m)
                    .WithMessage("La cantidad de UVT aplicada no puede ser negativa.")
                .Must(value => value <= MaxUvtQty)
                    .WithMessage($"La cantidad de UVT aplicada no puede superar {MaxUvtQty}.")
                .Must(value => GetScale(value) <= 2)
                    .WithMessage("La cantidad de UVT aplicada solo admite hasta 2 decimales.");

            RuleFor(x => x.UvtValueApplied)
                .Cascade(CascadeMode.Stop)
                .Must(value => value > 0m)
                    .WithMessage("El valor de la UVT debe ser mayor que cero.")
                .Must(value => value <= MaxMoney)
                    .WithMessage($"El valor de la UVT no puede superar {MaxMoney}.")
                .Must(value => GetScale(value) <= 2)
                    .WithMessage("El valor de la UVT solo admite hasta 2 decimales.");

            RuleFor(x => x.VatRateApplied)
                .Cascade(CascadeMode.Stop)
                .Must(value => value >= 0m)
                    .WithMessage("La tarifa de IVA no puede ser negativa.")
                .Must(value => value <= MaxVatRate)
                    .WithMessage("La tarifa de IVA no puede ser mayor que 1 (100%).")
                .Must(value => GetScale(value) <= 4)
                    .WithMessage("La tarifa de IVA solo admite hasta 4 decimales.");

            RuleFor(x => x.BaseAmount)
                .Cascade(CascadeMode.Stop)
                .Must(value => value >= 0m)
                    .WithMessage("El valor base no puede ser negativo.")
                .Must(value => value <= MaxMoney)
                    .WithMessage($"El valor base no puede superar {MaxMoney}.")
                .Must(value => GetScale(value) <= 2)
                    .WithMessage("El valor base solo admite hasta 2 decimales.");

            RuleFor(x => x.VatAmount)
                .Cascade(CascadeMode.Stop)
                .Must(value => value >= 0m)
                    .WithMessage("El valor de IVA no puede ser negativo.")
                .Must(value => value <= MaxMoney)
                    .WithMessage($"El valor de IVA no puede superar {MaxMoney}.")
                .Must(value => GetScale(value) <= 2)
                    .WithMessage("El valor de IVA solo admite hasta 2 decimales.");

            RuleFor(x => x.TotalAmount)
                .Cascade(CascadeMode.Stop)
                .Must(value => value >= 0m)
                    .WithMessage("El valor total no puede ser negativo.")
                .Must(value => value <= MaxMoney)
                    .WithMessage($"El valor total no puede superar {MaxMoney}.")
                .Must(value => GetScale(value) <= 2)
                    .WithMessage("El valor total solo admite hasta 2 decimales.");

            RuleFor(x => x.DaysLate)
                .Cascade(CascadeMode.Stop)
                .Must(value => value is null || value.Value >= 0)
                    .WithMessage("Los dias en mora no pueden ser negativos.")
                .Must(value => value is null || value.Value <= MaxDaysLate)
                    .WithMessage($"Los dias en mora no pueden superar {MaxDaysLate}.");

            RuleFor(x => x.LateAmount)
                .Cascade(CascadeMode.Stop)
                .Must(value => value is null || value.Value >= 0m)
                    .WithMessage("El valor de mora no puede ser negativo.")
                .Must(value => value is null || value.Value <= MaxMoney)
                    .WithMessage($"El valor de mora no puede superar {MaxMoney}.")
                .Must(value => value is null || GetScale(value.Value) <= 2)
                    .WithMessage("El valor de mora solo admite hasta 2 decimales.");

            RuleFor(x => x.Status)
                .Cascade(CascadeMode.Stop)
                .Must(value => !string.IsNullOrWhiteSpace(value))
                    .WithMessage("El estado es obligatorio.")
                .Must(value => AllowedStatuses.Contains(value!.Trim().ToUpperInvariant()))
                    .WithMessage(AllowedStatusesMessage);

            RuleFor(x => x)
                .Custom((dto, context) =>
                {
                    if (dto.DueDate != default && (dto.DueDate.Year != dto.Year || dto.DueDate.Month != dto.Month))
                    {
                        context.AddFailure(nameof(dto.DueDate), "La fecha de vencimiento debe pertenecer al periodo indicado.");
                    }

                    var expectedTotal = dto.BaseAmount + dto.VatAmount;
                    if (Math.Abs(expectedTotal - dto.TotalAmount) > DecimalTolerance)
                    {
                        context.AddFailure(nameof(dto.TotalAmount), "El total debe ser igual a la suma de la base y el IVA (tolerancia 0.01).");
                    }

                    if (dto.LateAmount.HasValue && dto.LateAmount.Value > 0m && dto.DaysLate.GetValueOrDefault() <= 0)
                    {
                        context.AddFailure(nameof(dto.DaysLate), "Cuando existe valor por mora, los dias en mora deben ser mayores a cero.");
                    }
                });
        }

        private static int GetScale(decimal value) => (decimal.GetBits(value)[3] >> 16) & 0x7F;
    }
}
