using Data.Interfaz.IDataImplemenent.SecurityAuthentication;
using Data.Repository;
using Entity.Domain.Models.Implements.SecurityAuthentication;
using Entity.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Data.Services.AdministratiosSystem
{
    public class RolFormPermissionRepository : DataGeneric<RolFormPermission>, IRolFormPermissionRepository
    {
        public RolFormPermissionRepository(ApplicationDbContext context) : base(context) { }

        public override async Task<IEnumerable<RolFormPermission>> GetAllAsync()
        {
            return await _dbSet.AsNoTracking()
                .Where(e => !e.IsDeleted)
                .Include(e => e.Rol)
                .Include(e => e.Form)
                .Include(e => e.Permission)
                .ToListAsync();
        }

        public async Task<IEnumerable<RolFormPermission>> GetByRolAndFormAsync(int rolId, int formId)
        {
            return await _dbSet
                .Where(e => e.RolId == rolId && e.FormId == formId && !e.IsDeleted)
                .ToListAsync();
        }

        public override async Task<RolFormPermission?> GetByIdAsync(int id)
        {
            return await _dbSet.AsNoTracking()
                .Include(e => e.Rol)
                .Include(e => e.Form)
                .Include(e => e.Permission)
                .FirstOrDefaultAsync(e => e.Id == id && !e.IsDeleted);
        }

        // Usuarios que están asignados a un rol (activos)
        public async Task<List<int>> GetUserIdsByRoleIdAsync(int roleId)
        {
            var q = _context.Set<RolUser>().AsNoTracking()
                .Where(ru => ru.RolId == roleId && ru.Active && !ru.IsDeleted)
                .Select(ru => ru.UserId)
                .Distinct();

            return await q.ToListAsync();
        }
    }
}
