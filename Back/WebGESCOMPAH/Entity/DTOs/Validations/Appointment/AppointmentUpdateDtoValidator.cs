using System.Text.RegularExpressions;
using Entity.DTOs.Implements.Business.Appointment;
using FluentValidation;

namespace Entity.DTOs.Validations.Appointment
{
    public class AppointmentUpdateDtoValidator : AppointmentBaseDtoValidator<AppointmentUpdateDto>
    {
        private static readonly Regex PhoneRegex = new(@"^3\d{9}$", RegexOptions.Compiled);
        private static readonly Regex EmailRegex = new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled);

        public AppointmentUpdateDtoValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0)
                    .WithMessage("El identificador es obligatorio.");

            RuleFor(x => x.FullName)
                .Cascade(CascadeMode.Stop)
                .Must(value => string.IsNullOrWhiteSpace(value) || value.Trim().Length <= 120)
                    .WithMessage("El nombre completo no puede superar 120 caracteres.");

            RuleFor(x => x.Email)
                .Cascade(CascadeMode.Stop)
                .Must(value => string.IsNullOrWhiteSpace(value) || EmailRegex.IsMatch(value.Trim()))
                    .WithMessage("El correo no tiene un formato valido.");

            RuleFor(x => x.Phone)
                .Cascade(CascadeMode.Stop)
                .Must(value => string.IsNullOrWhiteSpace(value) || PhoneRegex.IsMatch(value.Trim()))
                    .WithMessage("El telefono debe tener 10 digitos y comenzar por 3.");
        }
    }
}
