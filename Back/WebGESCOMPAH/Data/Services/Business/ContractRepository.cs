using Data.Interfaz.IDataImplement.Business;
using Data.Repository;
using Entity.Domain.Models.Implements.Business;
using Entity.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Data.Services.Business
{
    public class ContractRepository : DataGeneric<Contract>, IContractRepository
    {
        public ContractRepository(ApplicationDbContext context) : base(context)
        {
        }

        public override async Task<IEnumerable<Contract>> GetAllAsync()
        {
            return await _dbSet
                .Include(c => c.Person)
                    .ThenInclude(p => p.User)
                .Include(c => c.PremisesLeased)
                    .ThenInclude(pl => pl.Establishment)
                        .ThenInclude(e => e.Plaza)
                .AsNoTracking()
                .ToListAsync();
        }


        public override async Task<Contract?> GetByIdAsync(int id)
        {

            return await _dbSet
                    .Include(c => c.Person)
                        .ThenInclude(p => p.User) 
                    .Include(c => c.PremisesLeased)
                        .ThenInclude(pl => pl.Establishment)
                            .ThenInclude(e => e.Plaza)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(c => c.Id == id);

        }

    }
}
