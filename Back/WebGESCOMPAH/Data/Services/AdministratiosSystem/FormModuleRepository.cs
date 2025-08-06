using Data.Interfaz.IDataImplemenent.AdministrationSystem;
using Data.Repository;
using Entity.Domain.Models.Implements.AdministrationSystem;
using Entity.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Data.Services.AdministratiosSystem
{
    public class FormModuleRepository : DataGeneric<FormModule>, IFormModuleRepository
    {
        public FormModuleRepository(ApplicationDbContext context) : base(context)
        {
        }

        public override async Task<IEnumerable<FormModule>> GetAllAsync()
        {
            return await _dbSet
                .Include(e => e.Form)
                .Include(e=>e.Module)
                .ToListAsync();
        }

        public override async Task<FormModule?> GetByIdAsync(int id)
        {
            return await _dbSet.Include(e => e.Form).Include(e=>e.Module).FirstOrDefaultAsync(e => e.Id == id);
        }
    }
}
