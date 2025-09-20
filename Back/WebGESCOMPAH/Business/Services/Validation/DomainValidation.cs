using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Utilities.Exceptions;

namespace Business.Services.Validation
{
    internal static class DomainValidation
    {
        private static readonly Regex MultiSpaceRegex = new(@"\s+", RegexOptions.Compiled);
        private static readonly Regex AlphaHumanRegex = new(@"^[\p{L}\p{M}]+(?:[ '\-][\p{L}\p{M}]+)*$", RegexOptions.Compiled | RegexOptions.CultureInvariant);
        private static readonly Regex DocumentRegex = new(@"^\d{7,10}$", RegexOptions.Compiled | RegexOptions.CultureInvariant);
        private static readonly Regex PhoneRegex = new(@"^3\d{9}$", RegexOptions.Compiled | RegexOptions.CultureInvariant);
        private static readonly Regex AddressRegex = new(@"^[\p{L}\p{M}\d\s#\-,.]+$", RegexOptions.Compiled | RegexOptions.CultureInvariant);
        private static readonly Regex EmailRegex = new(@"^[^@\s]+@[^@\s]+\.[^@\s]{2,24}$", RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);
        private static readonly Regex NonDigitRegex = new(@"[^\d]", RegexOptions.Compiled);

        internal static string NormalizeWhitespace(string? value)
        {
            if (string.IsNullOrWhiteSpace(value)) return string.Empty;
            return MultiSpaceRegex.Replace(value.Trim(), " ");
        }

        internal static string RequireName(string? value, string fieldName, int maxLength)
        {
            var normalized = NormalizeWhitespace(value);
            if (string.IsNullOrEmpty(normalized))
                throw new BusinessException($"{fieldName} es obligatorio.");
            if (normalized.Length > maxLength)
                throw new BusinessException($"{fieldName} no puede superar {maxLength} caracteres.");
            if (!AlphaHumanRegex.IsMatch(normalized))
                throw new BusinessException($"{fieldName} contiene caracteres no permitidos.");
            return normalized;
        }

        internal static string RequireText(string? value, string fieldName, int maxLength)
        {
            var normalized = NormalizeWhitespace(value);
            if (string.IsNullOrEmpty(normalized))
                throw new BusinessException($"{fieldName} es obligatorio.");
            if (normalized.Length > maxLength)
                throw new BusinessException($"{fieldName} no puede superar {maxLength} caracteres.");
            return normalized;
        }

        internal static string RequireColombianDocument(string? value)
        {
            var normalized = NormalizeWhitespace(value);
            normalized = NonDigitRegex.Replace(normalized, string.Empty);
            if (!DocumentRegex.IsMatch(normalized))
                throw new BusinessException("El documento debe contener entre 7 y 10 dígitos.");
            return normalized;
        }

        internal static string RequireColombianPhone(string? value)
        {
            var normalized = NormalizeWhitespace(value);
            normalized = NonDigitRegex.Replace(normalized, string.Empty);
            if (!PhoneRegex.IsMatch(normalized))
                throw new BusinessException("El celular debe tener 10 dígitos y comenzar en 3.");
            return normalized;
        }

        internal static string NormalizeAddress(string? value, bool required, int maxLength)
        {
            var normalized = NormalizeWhitespace(value);
            if (string.IsNullOrEmpty(normalized))
            {
                if (required)
                    throw new BusinessException("La dirección es obligatoria.");
                return string.Empty;
            }

            if (normalized.Length > maxLength)
                throw new BusinessException($"La dirección no puede superar {maxLength} caracteres.");
            if (!AddressRegex.IsMatch(normalized))
                throw new BusinessException("La dirección contiene caracteres no permitidos.");
            return normalized;
        }

        internal static string? NormalizeEmail(string? email)
        {
            if (string.IsNullOrWhiteSpace(email)) return null;
            var normalized = email.Trim().ToLowerInvariant();
            var atIndex = normalized.IndexOf('@');
            if (atIndex > -1)
            {
                var localPart = normalized[..atIndex];
                var domainPart = normalized[(atIndex + 1)..];
                if (!string.IsNullOrEmpty(domainPart) && !domainPart.Contains('.'))
                {
                    normalized = $"{localPart}@{domainPart}.com";
                }
            }
            return normalized;
        }

        internal static void EnsureValidEmail(string? email)
        {
            if (string.IsNullOrWhiteSpace(email)) return;
            if (!EmailRegex.IsMatch(email))
                throw new BusinessException("El correo proporcionado no es válido.");
        }

        internal static void EnsureCityId(int cityId)
        {
            if (cityId <= 0)
                throw new BusinessException("Debe seleccionar una ciudad válida.");
        }

        internal static void EnsureId(int id, string fieldName)
        {
            if (id <= 0)
                throw new BusinessException($"{fieldName} inválido.");
        }

        internal static void EnsureDateRange(DateTime startDate, DateTime endDate, string startLabel, string endLabel)
        {
            if (endDate.Date < startDate.Date)
                throw new BusinessException($"{endLabel} no puede ser anterior a {startLabel}.");
        }

        internal static void EnsureStartNotInPast(DateTime startDate, string fieldLabel, TimeZoneInfo? timeZone = null)
        {
            var nowUtc = DateTime.UtcNow;
            var reference = timeZone is null ? nowUtc : TimeZoneInfo.ConvertTime(nowUtc, timeZone);
            if (startDate.Date < reference.Date)
                throw new BusinessException($"{fieldLabel} no puede ser anterior a la fecha actual.");
        }

        internal static decimal EnsureDecimalRange(decimal value, decimal min, decimal max, int maxDecimals, string fieldLabel)
        {
            if (value < min)
                throw new BusinessException($"{fieldLabel} no puede ser menor que {min}.");
            if (value > max)
                throw new BusinessException($"{fieldLabel} no puede superar {max}.");
            if (GetDecimalScale(value) > maxDecimals)
                throw new BusinessException($"{fieldLabel} admite máximo {maxDecimals} decimales.");
            return value;
        }

        internal static List<int> NormalizePositiveIds(IEnumerable<int>? source)
        {
            return source?.Where(id => id > 0).Distinct().ToList() ?? new List<int>();
        }

        private static int GetDecimalScale(decimal value)
        {
            var bits = decimal.GetBits(value);
            return (bits[3] >> 16) & 0x7F;
        }
    }
}
