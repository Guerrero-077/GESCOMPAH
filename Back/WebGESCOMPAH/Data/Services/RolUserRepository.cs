using Data.Interfaz.IDataImplemenent;
using Data.Repository;
using Entity.Domain.Models.Implements.SecurityAuthentication;
using Entity.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Data.Services
{
    public class RolUserRepository : DataGeneric<RolUser>, IRolUserRepository
    {
        public RolUserRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<RolUser> AsignateRolDefault(User user)
        {
            var rolUser = new RolUser
            {
                UserId = user.Id,
                RolId = 2,

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

    }
}
