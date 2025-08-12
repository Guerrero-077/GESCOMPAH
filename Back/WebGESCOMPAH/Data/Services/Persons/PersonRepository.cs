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
    }
}
