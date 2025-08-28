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
                .Include(e => e.User)
                .ToListAsync();
        }

     
        public override async Task<Person?> GetByIdAsync(int id)
        {
            return await _dbSet
                .Include(p => p.City)
                .Include(e => e.User)
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<bool> ExistsByDocumentAsync(string document)
        {
            return await _dbSet
                .AsNoTracking()
                .AnyAsync(p => p.Document == document && !p.IsDeleted);
        }

        public async Task<Person?> GetByDocumentAsync(string document)
        {
            return await _dbSet
                .Include(p => p.City)
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.Document == document && !p.IsDeleted);
        }

    }
}
