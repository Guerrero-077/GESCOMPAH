using FluentValidation;
using Entity.DTOs.Implements.Business.Appointment;

namespace Entity.DTOs.Validations.Appointment
{
    public abstract class AppointmentBaseDtoValidator<T> : AbstractValidator<T> where T : AppointmentBaseDto
    {
        public AppointmentBaseDtoValidator()
        {
            RuleFor(x => x.FullName)
                .NotEmpty().WithMessage("El nombre completo es obligatorio.")
                .MaximumLength(200);

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("El email es obligatorio.")
                .EmailAddress().WithMessage("Formato de email inválido.");

            RuleFor(x => x.Phone)
                .NotEmpty().WithMessage("El teléfono es obligatorio.")
                .Matches("^\\+\\d{7,15}$").WithMessage("Formato de teléfono inválido.");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("La descripción es obligatoria.")
                .MaximumLength(1000);

            RuleFor(x => x.EstablishmentId)
                .GreaterThan(0).WithMessage("Debe seleccionar un establecimiento válido.");

            RuleFor(x => x.RequestDate)
                .NotEqual(default(DateTime)).WithMessage("La fecha de solicitud es obligatoria.");

            RuleFor(x => x.DateTimeAssigned)
                .NotEqual(default(DateTime)).WithMessage("La fecha/hora asignada es obligatoria.")
                .GreaterThan(x => x.RequestDate).WithMessage("La fecha/hora asignada debe ser posterior a la fecha de solicitud.");
        }
    }
}
