using Business.Interfaces.IBusiness;
using Entity.DTOs.Implements.Business.Appointment;

namespace Business.Interfaces.Implements.Business
{
    public interface IAppointmentService : IBusiness<AppointmentSelectDto, AppointmentCreateDto, AppointmentUpdateDto>
    {
        Task<bool> ChangesStatusAsync(int id, int status);
        //Task<ContractCreateDto> AprovedAppointment(int id);
    }
}
