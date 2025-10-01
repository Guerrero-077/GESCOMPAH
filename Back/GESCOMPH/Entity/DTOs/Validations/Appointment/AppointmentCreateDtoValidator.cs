using System.Text.RegularExpressions;
using Entity.DTOs.Implements.Business.Appointment;
using FluentValidation;

namespace Entity.DTOs.Validations.Appointment
{
    public class AppointmentCreateDtoValidator : AppointmentBaseDtoValidator<AppointmentCreateDto>
    {
        private static readonly Regex NameRegex = new(@"^[\p{L}\p{M}\s'-]+$", RegexOptions.Compiled);
        private static readonly Regex DocumentRegex = new(@"^\d{7,12}$", RegexOptions.Compiled);
        private static readonly Regex PhoneRegex = new(@"^3\d{9}$", RegexOptions.Compiled);
        private static readonly Regex AddressRegex = new(@"^[\p{L}\p{M}\d\s#\-,.]+$", RegexOptions.Compiled);

        public AppointmentCreateDtoValidator()
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
                    .WithMessage("El documento debe tener entre 7 y 12 digitos.");

            RuleFor(x => x.Address)
                .Cascade(CascadeMode.Stop)
                .Must(value => string.IsNullOrWhiteSpace(value) || value.Trim().Length <= 150)
                    .WithMessage("La direccion no puede superar 150 caracteres.")
                .Must(value => string.IsNullOrWhiteSpace(value) || AddressRegex.IsMatch(value.Trim()))
                    .WithMessage("La direccion contiene caracteres no validos.");

            RuleFor(x => x.Phone)
                .Cascade(CascadeMode.Stop)
                .Must(value => !string.IsNullOrWhiteSpace(value))
                    .WithMessage("El telefono es obligatorio.")
                .Must(value => value != null && PhoneRegex.IsMatch(value.Trim()))
                    .WithMessage("El telefono debe tener 10 digitos y comenzar por 3.");

            RuleFor(x => x.CityId)
                .GreaterThan(0)
                    .WithMessage("Debe seleccionar una ciudad valida.");
        }
    }
}
