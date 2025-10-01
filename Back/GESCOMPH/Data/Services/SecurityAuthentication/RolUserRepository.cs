using Data.Interfaz.IDataImplement.SecurityAuthentication;
using Data.Repository;
using Entity.Domain.Models.Implements.SecurityAuthentication;
using Entity.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Data.Services.SecurityAuthentication
{
    public class RolUserRepository : DataGeneric<RolUser>, IRolUserRepository
    {
        public RolUserRepository(ApplicationDbContext context) : base(context)
        {
        }


        public override async Task<IEnumerable<RolUser>> GetAllAsync()
        {
            return await _dbSet
                .Include(e => e.Rol)
                .Include(e => e.User)
                .ToListAsync();
        }

        public override async Task<RolUser?> GetByIdAsync(int id)
        {
            return await _dbSet
                    .Include(e => e.Rol)
                    .Include(e => e.User)
                    .FirstOrDefaultAsync(e => e.Id == id);
        }
        public async Task<RolUser> AsignateRolDefault(User user)
        {
            var rolUser = new RolUser
            {
                UserId = user.Id,
                RolId = 2,
                Active = true,
            };

            _context.RolUsers.Add(rolUser);
            await _context.SaveChangesAsync();

            return rolUser;
        }
        public async Task<List<Rol>> GetRolesByUserIdAsync(int userId)
        {
            return await _context.RolUsers
                .Where(ru => ru.UserId == userId)
                .Include(ru => ru.Rol)
                .Select(ru => ru.Rol)
                .ToListAsync();
        }

        public async Task<IEnumerable<string>> GetRoleNamesByUserIdAsync(int userId)
        {
            return await _context.RolUsers
                .AsNoTracking()
                .Where(ru => ru.UserId == userId
                          && ru.Active
                          && !string.IsNullOrWhiteSpace(ru.Rol.Name))
                .Select(ru => ru.Rol.Name)
                .Distinct()
                .ToListAsync();
        }



        public async Task ReplaceUserRolesAsync(int userId, IEnumerable<int> roleIds)
        {
            var distinctIds = (roleIds ?? Enumerable.Empty<int>())
                .Where(id => id > 0)
                .Distinct()
                .ToList();

            // Traer roles actuales del usuario
            var current = await _context.RolUsers
                .Where(ru => ru.UserId == userId)
                .ToListAsync();

            // Calcula sets
            var currentIds = current.Select(c => c.RolId).ToHashSet();
            var toAdd = distinctIds.Where(id => !currentIds.Contains(id)).ToList();
            var toRemove = current.Where(c => !distinctIds.Contains(c.RolId)).ToList();

            // Quitar (hard delete); si prefieres soft, marca Active = false
            if (toRemove.Count > 0)
            {
                _context.RolUsers.RemoveRange(toRemove);
            }

            // Agregar
            foreach (var rid in toAdd)
            {
                _context.RolUsers.Add(new RolUser
                {
                    UserId = userId,
                    RolId = rid,
                    Active = true
                });
            }

            await _context.SaveChangesAsync();
        }



    }
}
