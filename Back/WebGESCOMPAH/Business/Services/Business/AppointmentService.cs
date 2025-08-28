using Business.Interfaces.Implements.Business;
using Business.Repository;
using Data.Interfaz.IDataImplemenent.Business;
using Entity.Domain.Models.Implements.Business;
using Entity.DTOs.Implements.Business.Appointment;
using MapsterMapper;
using Utilities.Helpers.Business;

namespace Business.Services.Business
{
    public class AppointmentService(IAppointmentRepository data, IMapper mapper) : BusinessGeneric<AppointmentSelectDto, AppointmentCreateDto, AppointmentUpdateDto, Appointment>(data, mapper), IAppointmentService
    {
        public async Task<bool> ChangesStatusAsync(int id, int status)
        {
            BusinessValidationHelper.ThrowIfZeroOrLess(id, "El ID debe ser mayor que cero.");
            return await data.ChangeStatusAsync(id, status);
        }
    }
}
