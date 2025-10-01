using FluentValidation;
using Entity.DTOs.Implements.Business.Appointment;

namespace Entity.DTOs.Validations.Appointment
{
    public class AppointmentCreateDtoValidator : AppointmentBaseDtoValidator<AppointmentCreateDto>
    {
        public AppointmentCreateDtoValidator()
        {
        }
    }
}
