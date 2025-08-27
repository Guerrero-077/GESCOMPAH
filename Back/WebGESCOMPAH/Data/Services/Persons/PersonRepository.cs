using Data.Interfaz.IDataImplement.Persons;
using Data.Repository;
using Entity.Domain.Models.Implements.Persons;
using Entity.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Data.Services.Persons
{
    public class PersonRepository : DataGeneric<Person>, IPersonRepository
    {
        public PersonRepository(ApplicationDbContext context) : base(context)
        {
        }

        public override async Task<IEnumerable<Person>> GetAllAsync()
        {
            return  await _dbSet
                .Include(e => e.City)
                .ToListAsync();
        }
        public async Task<Person?> GetByIdWithCityAsync(int id)
        {
            return await _dbSet
                .Include(p => p.City)
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<bool> ExistsByDocumentAsync(string document)
        {
            return await _dbSet
                .AsNoTracking()
                .AnyAsync(p => p.Document == document && !p.IsDeleted);
        }

        //public async Task<Person?> GetByDocumentAsync(string document)
        //{
        //    return await _dbSet
        //        .Include(p => p.City)
        //        .FirstOrDefaultAsync(p => p.Document == document && !p.IsDeleted);
        //}

        public async Task<Person?> GetByDocumentAsync(string document)
        {
            return await _dbSet
                .Include(p => p.City)
                .Include(p => p.User) // ← Agregado para obtener el Email
                .FirstOrDefaultAsync(p => p.Document == document && !p.IsDeleted);
        }

    }
}
