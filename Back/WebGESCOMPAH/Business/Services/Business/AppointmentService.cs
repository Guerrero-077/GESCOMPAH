using Business.Interfaces.Implements.Business;
using Business.Repository;
using Data.Interfaz.IDataImplement.Business;
using Entity.Domain.Models.Implements.Business;
using Entity.DTOs.Implements.Business.Appointment;
using MapsterMapper;

namespace Business.Services.Business
{
    public class AppointmentService(IAppointmentRepository data, IMapper mapper) : BusinessGeneric<AppointmentSelectDto, AppointmentCreateDto, AppointmentUpdateDto, Appointment>(data, mapper), IAppointmentService
    {
    }
}
