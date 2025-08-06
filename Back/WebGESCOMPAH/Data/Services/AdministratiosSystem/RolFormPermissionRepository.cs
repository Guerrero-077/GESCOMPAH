using Data.Interfaz.IDataImplemenent.SecurityAuthentication;
using Data.Repository;
using Entity.Domain.Models.Implements.SecurityAuthentication;
using Entity.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Data.Services.AdministratiosSystem
{
    public class RolFormPermissionRepository : DataGeneric<RolFormPermission>, IRolFormPermissionRepository
    {
        public RolFormPermissionRepository(ApplicationDbContext context) : base(context)
        {
        }

        public override async Task<IEnumerable<RolFormPermission>> GetAllAsync()
        {
            return await _dbSet
                .Include(e => e.Rol)
                .Include(e => e.Form)
                .Include(e => e.Permission)
                .ToListAsync();
        }

        public override async Task<RolFormPermission?> GetByIdAsync(int id)
        {
            return await _dbSet
                .Include(e=> e.Rol)
                .Include(e =>  e.Form)
                .Include(e=> e.Permission)
                .FirstOrDefaultAsync(e => e.Id == id);
        }
    }
}
