using Data.Interfaz.IDataImplement.Business;
using Data.Repository;
using Entity.Domain.Models.Implements.Business;
using Entity.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Data.Services.Business
{
    public class ContractRepository : DataGeneric<Contract>, IContractRepository
    {
        public ContractRepository(ApplicationDbContext context) : base(context) { }

        public override async Task<IEnumerable<Contract>> GetAllAsync()
        {
            return await _dbSet
                .Include(c => c.Person).ThenInclude(p => p.User)
                .Include(c => c.PremisesLeased).ThenInclude(pl => pl.Establishment).ThenInclude(e => e.Plaza)
                .AsNoTracking()
                .ToListAsync();
        }

        public override async Task<Contract?> GetByIdAsync(int id)
        {
            return await _dbSet
                .Include(c => c.Person).ThenInclude(p => p.User)
                .Include(c => c.PremisesLeased).ThenInclude(pl => pl.Establishment).ThenInclude(e => e.Plaza)
                .Include(c => c.ContractClauses)
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        ///Metodos adicionales para optener los contratos por persona 


        public async Task<IReadOnlyList<ContractCard>> GetCardsByPersonAsync(int personId) =>
            await _dbSet.AsNoTracking()
                .Where(c => !c.IsDeleted && c.PersonId == personId)
                .Select(c => new ContractCard(
                    c.Id, c.PersonId, c.StartDate, c.EndDate,
                    c.TotalBaseRentAgreed, c.TotalUvtQtyAgreed, c.Active))
                .ToListAsync();

                public async Task<IReadOnlyList<ContractCard>> GetCardsAllAsync() =>
                    await _dbSet.AsNoTracking()
                        .Where(c => !c.IsDeleted)
                        .Select(c => new ContractCard(
                            c.Id, c.PersonId, c.StartDate, c.EndDate,
                            c.TotalBaseRentAgreed, c.TotalUvtQtyAgreed, c.Active))
                        .ToListAsync();

    }
}
