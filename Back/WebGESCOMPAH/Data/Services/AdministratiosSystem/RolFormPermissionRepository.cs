using Data.Interfaz.IDataImplement.SecurityAuthentication;
using Data.Repository;
using Entity.Domain.Models.Implements.SecurityAuthentication;
using Entity.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Data.Repositories.Implementations.SecurityAuthentication
{
    public class RolFormPermissionRepository : DataGeneric<RolFormPermission>, IRolFormPermissionRepository
    {
        public RolFormPermissionRepository(ApplicationDbContext context) : base(context) { }

        public override async Task<IEnumerable<RolFormPermission>> GetAllAsync()
        {
            return await _dbSet.AsNoTracking()
                .Where(e => !e.IsDeleted)
                .OrderByDescending(e => e.CreatedAt)
                .ThenByDescending(e => e.Id)
                .Include(e => e.Rol)
                .Include(e => e.Form)
                .Include(e => e.Permission)
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

        public async Task<IEnumerable<RolFormPermission>> GetByRolAndFormAsync(int rolId, int formId)
        {
            return await _dbSet
                .Where(e => e.RolId == rolId && e.FormId == formId && !e.IsDeleted)
                .ToListAsync();
        }

        public async Task<List<int>> GetUserIdsByRoleIdAsync(int rolId)
        {
            return await _context.Set<RolUser>()
                .AsNoTracking()
                .Where(ru => ru.RolId == rolId && ru.Active && !ru.IsDeleted)
                .Select(ru => ru.UserId)
                .Distinct()
                .ToListAsync();
        }
    }
}
