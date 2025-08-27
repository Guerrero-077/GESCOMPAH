using Data.Interfaz.IDataImplement.Business;
using Data.Repository;
using Entity.Domain.Models.Implements.Business;
using Entity.Infrastructure.Context;

namespace Data.Services.Business
{
    public class PremisesLeasedRepository : DataGeneric<PremisesLeased>, IPremisesLeasedRepository
    {
        public PremisesLeasedRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task AddRangeAsync(IEnumerable<PremisesLeased> entities)
        {
            await _dbSet.AddRangeAsync(entities);
            await _context.SaveChangesAsync();
        }
    }
}
