using Data.Interfaz.IDataImplement.SecurityAuthentication;
using Data.Repository;
using Entity.Domain.Models.Implements.SecurityAuthentication;
using Entity.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Data.Services.SecurityAuthentication
{
    public class UserRepository : DataGeneric<User>, IUserRepository
    {
        public UserRepository(ApplicationDbContext context) : base(context) { }

        // Usado por consultas genéricas (paginadas) para asegurar Includes necesarios.
        public override IQueryable<User> GetAllQueryable()
        {
            return _dbSet
                .AsNoTracking()
                .Where(u => !u.IsDeleted)
                .OrderByDescending(e => e.CreatedAt)
                .ThenByDescending(e => e.Id)
                .Include(u => u.Person)
                    .ThenInclude(p => p.City)
                .Include(u => u.RolUsers)
                    .ThenInclude(ru => ru.Rol);
        }

        // ========== LISTADOS ==========
        // SELECT masivo → NoTracking para no saturar el ChangeTracker
        public override async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _dbSet
                .AsNoTracking()
                .Where(u => !u.IsDeleted)
                .OrderByDescending(e => e.CreatedAt)
                .ThenByDescending(e => e.Id)
                .Include(u => u.Person)
                    .ThenInclude(p => p.City)
                .ToListAsync();
        }

        // ========== POR ID ==========
        // Para escenarios de UPDATE suele bastar sin includes (entidad tracked).
        // Mantengo tu override, pero recomiendo reducir includes si no son necesarios.
        public override async Task<User?> GetByIdAsync(int id)
        {
            return await _dbSet
                .FirstOrDefaultAsync(u => u.Id == id && !u.IsDeleted);
        }
        public async Task<User?> GetByIdForUpdateAsync(int id)
        {
            return await _dbSet
                .Include(u => u.Person) // tracked
                .FirstOrDefaultAsync(u => u.Id == id && !u.IsDeleted);
        }


        // Para SELECT detallado (salida al cliente)
        public async Task<User?> GetByIdWithDetailsAsync(int id)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(u => u.Person)
                    .ThenInclude(p => p.City)
                .Include(u => u.RolUsers)
                    .ThenInclude(ru => ru.Rol)
                .FirstOrDefaultAsync(u => u.Id == id && !u.IsDeleted);
        }

        // ========== BÚSQUEDAS POR EMAIL ==========
        public async Task<bool> ExistsByEmailAsync(string email)
        {
            var e = email.Trim().ToLower();
            return await _dbSet.AnyAsync(u => !u.IsDeleted && u.Email.ToLower() == e);
        }

        public async Task<bool> ExistsByEmailExcludingIdAsync(int id, string email)
        {
            var e = email.Trim().ToLower();
            return await _dbSet.AnyAsync(u => !u.IsDeleted && u.Id != id && u.Email.ToLower() == e);
        }

        public async Task<int?> GetIdByEmailAsync(string email)
        {
            var e = email.Trim().ToLower();
            return await _dbSet
                .Where(u => !u.IsDeleted && u.Email.ToLower() == e)
                .Select(u => (int?)u.Id)
                .FirstOrDefaultAsync();
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            var e = email.Trim().ToLower();
            return await _dbSet
                .Include(u => u.Person)
                .Include(u => u.RolUsers).ThenInclude(ru => ru.Rol)
                .FirstOrDefaultAsync(u => !u.IsDeleted && u.Email.ToLower() == e);
        }

        // Proyección mínima (read-only)
        public async Task<User?> GetByEmailProjectionAsync(string email)
        {
            var e = email.Trim().ToLower();
            return await _dbSet
                .AsNoTracking()
                .Where(u => !u.IsDeleted &&  u.Email.ToLower() == e)
                .Select(u => new User
                {
                    Id = u.Id,
                    Email = u.Email,
                    Password = u.Password,
                    PersonId = u.PersonId,
                    Active = u.Active,
                    IsDeleted = u.IsDeleted
                })
                .FirstOrDefaultAsync();
        }

        // ========== LOGIN (solo datos, sin lógica de verificación) ==========
        // Devuelve lo mínimo para que el servicio verifique hash, bloquee, etc.
        public async Task<User?> GetAuthUserByEmailAsync(string email)
        {
            var e = email.Trim().ToLower();
            return await _dbSet
                .AsNoTracking()
                .Where(u => !u.IsDeleted && u.Email.ToLower() == e)
                .Select(u => new User
                {
                    Id = u.Id,
                    Email = u.Email,
                    Password = u.Password, // hash
                    PersonId = u.PersonId
                })
                .FirstOrDefaultAsync();
        }

        // ========== PERSONA ==========
        public async Task<User?> GetByPersonIdAsync(int personId)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(u => u.Person)
                .Include(u => u.RolUsers)
                    .ThenInclude(ru => ru.Rol)
                .FirstOrDefaultAsync(u => u.PersonId == personId && !u.IsDeleted);
        }
    }
}
