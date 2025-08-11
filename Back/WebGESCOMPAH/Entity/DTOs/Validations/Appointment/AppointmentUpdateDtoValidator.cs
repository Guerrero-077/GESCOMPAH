using FluentValidation;
using Entity.DTOs.Implements.Business.Appointment;

namespace Entity.DTOs.Validations.Appointment
{
    public class AppointmentUpdateDtoValidator : AppointmentBaseDtoValidator<AppointmentUpdateDto>
    {
        public AppointmentUpdateDtoValidator()
        {
            RuleFor(x => x.Id).GreaterThan(0);
        }
    }
}
