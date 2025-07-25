using Business.Interfaces.Implements.Business;
using Business.Repository;
using Data.Interfaz.DataBasic;
using Entity.Domain.Models.Implements.Business;
using Entity.DTOs.Implements.Business.Appointment;
using MapsterMapper;

namespace Business.Services.Business
{
    public class AppointmentService : BusinessGeneric<AppointmentSelectDto, AppointmentCreateDto, AppointmentUpdateDto, Appointment>, IAppointmentService
    {
        public AppointmentService(IDataGeneric<Appointment> data, IMapper mapper) : base(data, mapper)
        {
        }
    }
}
