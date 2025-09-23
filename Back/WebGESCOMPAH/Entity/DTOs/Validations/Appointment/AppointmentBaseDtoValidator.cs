using System;
using Entity.DTOs.Implements.Business.Appointment;
using FluentValidation;

namespace Entity.DTOs.Validations.Appointment
{
    public abstract class AppointmentBaseDtoValidator<T> : AbstractValidator<T> where T : IAppointmentDto
    {
        protected AppointmentBaseDtoValidator()
        {
            RuleFor(x => x.Description)
                .Cascade(CascadeMode.Stop)
                .Must(value => !string.IsNullOrWhiteSpace(value))
                    .WithMessage("La descripcion es obligatoria.")
                .MaximumLength(1000)
                    .WithMessage("La descripcion no puede superar 1000 caracteres.");

            RuleFor(x => x.EstablishmentId)
                .GreaterThan(0)
                    .WithMessage("Debe seleccionar un establecimiento valido.");

            RuleFor(x => x.RequestDate)
                .NotEqual(default(DateTime))
                    .WithMessage("La fecha de solicitud es obligatoria.");

            RuleFor(x => x.DateTimeAssigned)
                .NotEqual(default(DateTime))
                    .WithMessage("La fecha y hora asignada es obligatoria.")
                .GreaterThan(x => x.RequestDate)
                    .WithMessage("La fecha y hora asignada debe ser posterior a la fecha de solicitud.");
        }
    }
}
