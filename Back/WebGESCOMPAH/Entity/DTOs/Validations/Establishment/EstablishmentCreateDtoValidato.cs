using System;
using System.Text.RegularExpressions;
using Entity.DTOs.Implements.Business.EstablishmentDto;
using FluentValidation;

namespace Entity.DTOs.Validations.Establishment
{
    public class EstablishmentCreateDtoValidator : AbstractValidator<EstablishmentCreateDto>
    {
        private const int NameMaxLength = 100;
        private const int DescriptionMaxLength = 500;
        private const int AddressMaxLength = 150;
        private static readonly Regex AddressRegex = new(@"^[\p{L}\p{M}\d\s#\-,.]+$", RegexOptions.Compiled);

        public EstablishmentCreateDtoValidator()
        {
            RuleFor(x => x)
                .Cascade(CascadeMode.Stop)
                .Custom((dto, context) =>
                {
                    if (dto is null)
                    {
                        context.AddFailure("Establishment", "Payload inválido.");
                        return;
                    }

                    var (name, nameError) = ValidateRequiredText(dto.Name, NameMaxLength);
                    if (nameError is not null) context.AddFailure(nameof(dto.Name), nameError);
                    dto.Name = name;

                    var (description, descriptionError) = ValidateRequiredText(dto.Description, DescriptionMaxLength);
                    if (descriptionError is not null) context.AddFailure(nameof(dto.Description), descriptionError);
                    dto.Description = description;

                    var (address, addressError) = ValidateOptionalAddress(dto.Address);
                    if (addressError is not null) context.AddFailure(nameof(dto.Address), addressError);
                    dto.Address = address;

                    var areaError = ValidateDecimal(dto.AreaM2, 1m, 1_000_000m);
                    if (areaError is not null) context.AddFailure(nameof(dto.AreaM2), areaError);

                    var uvtError = ValidateDecimal(dto.UvtQty, 1m, 9_999m);
                    if (uvtError is not null) context.AddFailure(nameof(dto.UvtQty), uvtError);

                    if (dto.PlazaId <= 0)
                        context.AddFailure(nameof(dto.PlazaId), "Debes seleccionar una plaza válida.");
                });
        }

        private static (string Sanitized, string? Error) ValidateRequiredText(string? value, int maxLength)
        {
            var trimmed = (value ?? string.Empty).Trim();
            if (string.IsNullOrEmpty(trimmed))
                return (trimmed, "Este campo es obligatorio.");
            if (trimmed.Length > maxLength)
                return (trimmed, $"No puede superar {maxLength} caracteres.");
            return (trimmed, null);
        }

        private static (string Sanitized, string? Error) ValidateOptionalAddress(string? value)
        {
            var trimmed = (value ?? string.Empty).Trim();
            if (string.IsNullOrEmpty(trimmed))
                return (string.Empty, null);
            if (trimmed.Length > AddressMaxLength)
                return (trimmed, $"La dirección no puede superar {AddressMaxLength} caracteres.");
            if (!AddressRegex.IsMatch(trimmed))
                return (trimmed, "La dirección contiene caracteres no permitidos.");
            return (trimmed, null);
        }

        private static string? ValidateDecimal(decimal value, decimal min, decimal max)
        {
            if (value < min)
                return $"El valor debe ser mayor o igual a {min}.";
            if (value > max)
                return $"El valor no puede superar {max}.";
            if (GetScale(value) > 2)
                return "Solo se permiten hasta 2 decimales.";
            return null;
        }

        private static int GetScale(decimal value) => (decimal.GetBits(value)[3] >> 16) & 0x7F;
    }
}
