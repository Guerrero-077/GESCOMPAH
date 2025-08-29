using Data.Interfaz.IDataImplement.SecurityAuthentication;
using Data.Repository;
using Entity.Domain.Models.Implements.SecurityAuthentication;
using Entity.DTOs.Implements.SecurityAuthentication.Auth;
using Entity.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Data.Services.SecurityAuthentication
    {
        public class UserRepository : DataGeneric<User>, IUserRepository
        {
            public UserRepository(ApplicationDbContext context) : base(context)
            {
            }

        public override async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _dbSet
                .Where(u => !u.IsDeleted)
                .Include(u => u.Person)
                .ThenInclude(p => p.City)
                .ToListAsync();
        }

        public override async Task<User?> GetByIdAsync(int id)
        {
            return await _dbSet
                .Include(u => u.Person)
                    .ThenInclude(p => p.City)
                .Include(u => u.RolUsers)
                    .ThenInclude(ru => ru.Rol)
                .FirstOrDefaultAsync(u => u.Id == id && u.IsDeleted == false);
        }

            public async Task<User> LoginUser(LoginDto loginDto)
            {
                bool suceeded = false;

                var user = await _dbSet
                    .FirstOrDefaultAsync(u =>
                                u.Email == loginDto.Email &&
                                u.Password == loginDto.Password);

                suceeded = user != null ? true : throw new UnauthorizedAccessException("Credenciales inválidas");

                return user;
            }



            public async Task<bool> ExistsByEmailAsync(string email)
            {
                return await _dbSet.AnyAsync(u => u.Email == email && u.IsDeleted == false);
            }

        public async Task<User?> GetByEmailProjectionAsync(string email)
        {
            return await _dbSet.AsNoTracking()
                .Where(u => u.Email == email && u.Active && !u.IsDeleted)
                .Select(u => new User
                {
                    Id = u.Id,
                    Email = u.Email,
                    Password = u.Password,
                    PersonId = u.PersonId
                })
                .FirstOrDefaultAsync();
        }

        public async Task<User?> GetByEmailAsync(string email)
            {
                return await _dbSet
                    .Include(u => u.Person)
                    .Include(u => u.RolUsers).ThenInclude(ru => ru.Rol)
                    .FirstOrDefaultAsync(u => u.Email == email && u.IsDeleted == false);
            }

        public async Task<User?> GetByPersonIdAsync(int personId)
        {
            return await _dbSet
                .Include(u => u.Person)
                .Include(u => u.RolUsers)
                    .ThenInclude(ru => ru.Rol)
                .FirstOrDefaultAsync(u => u.PersonId == personId && !u.IsDeleted);
        }

    }
}
