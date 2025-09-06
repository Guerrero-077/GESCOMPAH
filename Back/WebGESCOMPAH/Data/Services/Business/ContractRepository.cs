using Data.Interfaz.IDataImplement.Business;
using Data.Repository;
using Entity.Domain.Models.Implements.Business;
using Entity.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Data.Services.Business
{
    public class ContractRepository : DataGeneric<Contract>, IContractRepository
    {
        public ContractRepository(ApplicationDbContext context) : base(context) { }

        // ⚠️ Mantén este override para casos de detalle/export (no para grillas masivas)
        public override async Task<IEnumerable<Contract>> GetAllAsync()
        {
            return await _dbSet
                .OrderByDescending(e => e.CreatedAt)
                .ThenByDescending(e => e.Id)
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

        // ============== PROYECCIONES PARA GRID (sin Include) ==============

        public async Task<IReadOnlyList<ContractCard>> GetCardsByPersonAsync(int personId) =>
            await _dbSet.AsNoTracking()
                .Where(c => !c.IsDeleted && c.PersonId == personId)
                .OrderByDescending(e => e.CreatedAt)
                .ThenByDescending(e => e.Id)
                .Select(c => new ContractCard(
                    c.Id,
                    c.PersonId,
                    (c.Person.FirstName + " " + c.Person.LastName).Trim(),
                    c.Person.Document,
                    c.Person.Phone,
                    c.Person.User != null ? c.Person.User.Email : null,
                    c.StartDate,
                    c.EndDate,
                    c.TotalBaseRentAgreed,
                    c.TotalUvtQtyAgreed,
                    c.Active
                ))
                .ToListAsync();

        public async Task<IReadOnlyList<ContractCard>> GetCardsAllAsync() =>
            await _dbSet.AsNoTracking()
                .Where(c => !c.IsDeleted)
                .OrderByDescending(c => c.Id)
                .Select(c => new ContractCard(
                    c.Id,
                    c.PersonId,
                    (c.Person.FirstName + " " + c.Person.LastName).Trim(),
                    c.Person.Document,
                    c.Person.Phone,
                    c.Person.User != null ? c.Person.User.Email : null,
                    c.StartDate,
                    c.EndDate,
                    c.TotalBaseRentAgreed,
                    c.TotalUvtQtyAgreed,
                    c.Active
                ))
                .ToListAsync();

    }
}
