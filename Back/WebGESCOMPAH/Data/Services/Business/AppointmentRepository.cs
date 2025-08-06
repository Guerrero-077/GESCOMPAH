using Data.Interfaz.IDataImplemenent.Business;
using Data.Repository;
using Entity.Domain.Models.Implements.Business;
using Entity.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Data.Services.Business
{
    public class AppointmentRepository(ApplicationDbContext context) : DataGeneric<Appointment>(context), IAppointmentRepository
    {

        public override async Task<IEnumerable<Appointment>> GetAllAsync()
        {
            return await _dbSet
                .Where(e => !e.IsDeleted)
                .Include(e => e.Establishment)
                .ToListAsync();
        }

        public override async Task<Appointment?> GetByIdAsync(int id)
        {
            return await _dbSet
                .Where(e => e.Id == id && !e.IsDeleted)
                .Include (e => e.Establishment)
                .FirstOrDefaultAsync();

        }
    }
}
