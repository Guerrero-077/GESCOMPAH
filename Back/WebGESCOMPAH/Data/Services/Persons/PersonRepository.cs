using Data.Interfaz.IDataImplemenent.Persons;
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

    }
}
