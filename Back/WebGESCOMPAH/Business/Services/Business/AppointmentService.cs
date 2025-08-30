using Business.Interfaces.Implements.Business;
using Business.Repository;
using Data.Interfaz.IDataImplement.Business;
using Entity.Domain.Models.Implements.Business;
using Entity.DTOs.Implements.Business.Appointment;
using Entity.DTOs.Implements.Business.Contract;
using MapsterMapper;
using Utilities.Helpers.Business;

namespace Business.Services.Business
{
    public class AppointmentService(IAppointmentRepository data, IMapper mapper) : BusinessGeneric<AppointmentSelectDto, AppointmentCreateDto, AppointmentUpdateDto, Appointment>(data, mapper), IAppointmentService
    {
        public Task<ContractCreateDto> AprovedAppointment(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> ChangesStatusAsync(int id, int status)
        {
            BusinessValidationHelper.ThrowIfZeroOrLess(id, "El ID debe ser mayor que cero.");
            return await data.ChangeStatusAsync(id, status);
        }
    }
}
