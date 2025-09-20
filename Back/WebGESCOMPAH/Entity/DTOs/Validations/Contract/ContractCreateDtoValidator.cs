using System;
using System.Linq;
using System.Net.Mail;
using System.Text.RegularExpressions;
using Entity.DTOs.Implements.Business.Contract;
using FluentValidation;

namespace Entity.DTOs.Validations.Contract
{
    public class ContractCreateDtoValidator : AbstractValidator<ContractCreateDto>
    {
        private static readonly Regex NameRegex = new(@"^[\p{L}\p{M}]+(?:[ '\-][\p{L}\p{M}]+)*$", RegexOptions.Compiled);
        private static readonly Regex AddressRegex = new(@"^[\p{L}\p{M}\d\s#\-,.]+$", RegexOptions.Compiled);
        private static readonly Regex DocumentRegex = new(@"^\d{7,10}$", RegexOptions.Compiled);
        private static readonly Regex PhoneRegex = new(@"^3\d{9}$", RegexOptions.Compiled);
        private static readonly TimeZoneInfo BogotaTimeZone = ResolveBogotaTimeZone();

        public ContractCreateDtoValidator()
        {
            RuleFor(x => x.FirstName)
                .Cascade(CascadeMode.Stop)
                .Must(value => !string.IsNullOrWhiteSpace(value))
                    .WithMessage("El nombre es obligatorio.")
                .Must(value => value == null || value.Trim().Length <= 50)
                    .WithMessage("El nombre no puede superar 50 caracteres.")
                .Must(value => value == null || NameRegex.IsMatch(value.Trim()))
                    .WithMessage("El nombre contiene caracteres no validos.");

            RuleFor(x => x.LastName)
                .Cascade(CascadeMode.Stop)
                .Must(value => !string.IsNullOrWhiteSpace(value))
                    .WithMessage("El apellido es obligatorio.")
                .Must(value => value == null || value.Trim().Length <= 50)
                    .WithMessage("El apellido no puede superar 50 caracteres.")
                .Must(value => value == null || NameRegex.IsMatch(value.Trim()))
                    .WithMessage("El apellido contiene caracteres no validos.");

            RuleFor(x => x.Document)
                .Cascade(CascadeMode.Stop)
                .Must(value => !string.IsNullOrWhiteSpace(value))
                    .WithMessage("El documento es obligatorio.")
                .Must(value => value != null && DocumentRegex.IsMatch(value.Trim()))
                    .WithMessage("El documento debe contener entre 7 y 10 digitos.");

            RuleFor(x => x.Phone)
                .Cascade(CascadeMode.Stop)
                .Must(value => !string.IsNullOrWhiteSpace(value))
                    .WithMessage("El telefono es obligatorio.")
                .Must(value => value != null && PhoneRegex.IsMatch(value.Trim()))
                    .WithMessage("El telefono debe tener 10 digitos y comenzar por 3.");

            RuleFor(x => x.Address)
                .Cascade(CascadeMode.Stop)
                .Must(value => !string.IsNullOrWhiteSpace(value))
                    .WithMessage("La direccion es obligatoria.")
                .Must(value => value == null || value.Trim().Length <= 150)
                    .WithMessage("La direccion no puede superar 150 caracteres.")
                .Must(value => value == null || AddressRegex.IsMatch(value.Trim()))
                    .WithMessage("La direccion contiene caracteres no validos.");

            RuleFor(x => x.CityId)
                .GreaterThan(0)
                    .WithMessage("Debe seleccionar una ciudad valida.");

            RuleFor(x => x.Email)
                .Cascade(CascadeMode.Stop)
                .Must(IsValidEmail)
                    .WithMessage("El correo no tiene un formato valido.")
                .Must(HasValidEmailDomain)
                    .WithMessage("El dominio debe contener un punto.")
                .Must(HasValidEmailTld)
                    .WithMessage("La terminacion del dominio es invalida.")
                .When(x => !string.IsNullOrWhiteSpace(x.Email));

            RuleFor(x => x.StartDate)
                .Cascade(CascadeMode.Stop)
                .NotEqual(default(DateTime)).WithMessage("La fecha de inicio es obligatoria.")
                .Must(IsOnOrAfterToday)
                    .WithMessage("La fecha de inicio no puede ser anterior a la fecha actual.");

            RuleFor(x => x.EndDate)
                .Cascade(CascadeMode.Stop)
                .NotEqual(default(DateTime)).WithMessage("La fecha de finalizacion es obligatoria.")
                .GreaterThanOrEqualTo(x => x.StartDate)
                    .WithMessage("La fecha de finalizacion no puede ser anterior a la fecha de inicio.");

            RuleFor(x => x.EstablishmentIds)
                .Cascade(CascadeMode.Stop)
                .Must(list => list is { Count: > 0 })
                    .WithMessage("Debe seleccionar al menos un establecimiento.")
                .Must(list => list != null && list.All(id => id > 0))
                    .WithMessage("Todos los identificadores de establecimientos deben ser positivos.")
                .Must(list => list != null && list.Distinct().Count() == list.Count)
                    .WithMessage("No se permiten establecimientos duplicados.");

            RuleFor(x => x.ClauseIds)
                .Must(list => list == null || list.All(id => id > 0))
                    .WithMessage("Las clausulas deben tener identificadores positivos.")
                .Must(list => list == null || list.Distinct().Count() == list.Count)
                    .WithMessage("No se permiten clausulas duplicadas.");
        }

        private static bool IsValidEmail(string? email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return true;
            }

            var normalized = email.Trim().ToLowerInvariant();
            try
            {
                _ = new MailAddress(normalized);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private static bool HasValidEmailDomain(string? email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return true;
            }

            var normalized = email.Trim().ToLowerInvariant();
            var domain = normalized.Split('@').Skip(1).FirstOrDefault();
            return !string.IsNullOrWhiteSpace(domain) && domain.Contains('.');
        }

        private static bool HasValidEmailTld(string? email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return true;
            }

            var normalized = email.Trim().ToLowerInvariant();
            var domain = normalized.Split('@').Skip(1).FirstOrDefault();
            if (string.IsNullOrWhiteSpace(domain) || !domain.Contains('.'))
            {
                return false;
            }

            var tld = domain.Split('.').Last();
            return tld.Length is >= 2 and <= 24;
        }

        private static bool IsOnOrAfterToday(DateTime startDate)
        {
            var today = TimeZoneInfo.ConvertTime(DateTime.UtcNow, BogotaTimeZone).Date;
            return startDate.Date >= today;
        }

        private static TimeZoneInfo ResolveBogotaTimeZone()
        {
            try
            {
                return TimeZoneInfo.FindSystemTimeZoneById("America/Bogota");
            }
            catch (TimeZoneNotFoundException)
            {
                try
                {
                    return TimeZoneInfo.FindSystemTimeZoneById("SA Pacific Standard Time");
                }
                catch
                {
                    return TimeZoneInfo.Utc;
                }
            }
            catch
            {
                return TimeZoneInfo.Utc;
            }
        }
    }
}
