using Data.Interfaz.DataBasic;
using Entity.Domain.Models.Implements.Business;

namespace Data.Interfaz.IDataImplement.Business
{
    public interface IAppointmentRepository : IDataGeneric<Appointment>
    {
        Task<bool> ChangeStatusAsync(int id, int status);
    }
}
