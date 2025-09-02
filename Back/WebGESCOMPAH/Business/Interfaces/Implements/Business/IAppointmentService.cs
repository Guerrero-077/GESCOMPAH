using Business.Interfaces.IBusiness;
using Entity.DTOs.Implements.Business.Appointment;
using Entity.DTOs.Implements.Business.Contract;

namespace Business.Interfaces.Implements.Business
{
    public interface IAppointmentService : IBusiness<AppointmentSelectDto, AppointmentCreateDto, AppointmentUpdateDto>
    {
        Task<bool> RejectedAppointment(int id);
        //Task<ContractCreateDto> AproveAppointment(int id);
    }
}
