using Data.Interfaz.DataBasic;
using Entity.Domain.Models.Implements.Business;

namespace Data.Interfaz.IDataImplement.Business
{
    public interface IAppointmentRepository : IDataGeneric<Appointment>
    {
        Task<bool> RejectedAppointment(int id);
        //Task<bool> AssignedAppointment(int id, DateTime dateTimeAssigned);

    }
}
