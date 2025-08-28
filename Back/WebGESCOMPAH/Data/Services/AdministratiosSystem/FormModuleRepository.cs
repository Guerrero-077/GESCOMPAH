using Data.Interfaz.IDataImplement.AdministrationSystem;
using Data.Repository;
using Entity.Domain.Models.Implements.AdministrationSystem;
using Entity.Domain.Models.Implements.SecurityAuthentication;
using Entity.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Data.Services.AdministratiosSystem
{
    public class FormModuleRepository : DataGeneric<FormModule>, IFormModuleRepository
    {
        public FormModuleRepository(ApplicationDbContext context) : base(context) { }

        public override async Task<IEnumerable<FormModule>> GetAllAsync()
        {
            return await _dbSet.AsNoTracking()
                .Where(e => !e.IsDeleted)
                .Include(e => e.Form)
                .Include(e => e.Module)
                .ToListAsync();
        }

        public override async Task<FormModule?> GetByIdAsync(int id)
        {
            return await _dbSet.AsNoTracking()
                .Include(e => e.Form)
                .Include(e => e.Module)
                .FirstOrDefaultAsync(e => e.Id == id && !e.IsDeleted);
        }

        // Usuarios que tienen permisos (vía roles) sobre un Form dado
        public async Task<List<int>> GetUserIdsByFormIdAsync(int formId)
        {
            // rfp (activos) -> roles (activos) -> rolUsers (activos) -> users
            var q = from rfp in _context.Set<RolFormPermission>().AsNoTracking()
                    where rfp.FormId == formId && rfp.Active && !rfp.IsDeleted
                    join r in _context.Set<Rol>().AsNoTracking()
                        on rfp.RolId equals r.Id
                    where r.Active && !r.IsDeleted
                    join ru in _context.Set<RolUser>().AsNoTracking()
                        on r.Id equals ru.RolId
                    where ru.Active && !ru.IsDeleted
                    select ru.UserId;

            return await q.Distinct().ToListAsync();
        }
    }
}
