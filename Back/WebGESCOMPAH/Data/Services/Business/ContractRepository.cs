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
                .Include(c => c.ContractClauses).ThenInclude(cc => cc.Clause)
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








        /// <summary>
        /// Desactiva contratos con EndDate &lt; utcNow y que sigan activos.
        /// </summary>
        public async Task<IReadOnlyList<int>> DeactivateExpiredAsync(DateTime utcNow)
        {
            var ids = await _dbSet
                .Where(c => !c.IsDeleted && c.Active && c.EndDate < utcNow)
                .Select(c => c.Id)
                .ToListAsync();

            if (ids.Count == 0) return ids;

            await _dbSet
                .Where(c => ids.Contains(c.Id))
                .ExecuteUpdateAsync(s => s.SetProperty(c => c.Active, false));

            return ids;
        }


        /// <summary>
        /// Libera establecimientos de esos contratos expirados SI no están ocupados por otro contrato activo.
        /// </summary>
        public async Task<int> ReleaseEstablishmentsForExpiredAsync(DateTime utcNow)
        {
            // Contratos inactivos cuya fecha ya pasó (defensivo)
            var expiredIds = await _dbSet
                .Where(c => !c.IsDeleted && !c.Active && c.EndDate < utcNow)
                .Select(c => c.Id)
                .ToListAsync();

            if (expiredIds.Count == 0) return 0;

            // Establecimientos vinculados a contratos expirados
            var estIds = await _context.Set<PremisesLeased>()
                .Where(p => expiredIds.Contains(p.ContractId))
                .Select(p => p.EstablishmentId)
                .Distinct()
                .ToListAsync();

            if (estIds.Count == 0) return 0;

            // Establecimientos SIN otro contrato activo que los mantenga ocupados
            var estToActivate = await _context.Set<PremisesLeased>()
                .Where(pl => estIds.Contains(pl.EstablishmentId))
                .GroupBy(pl => pl.EstablishmentId)
                .Where(g => !g.Any(pl =>
                    _dbSet.Any(c => !c.IsDeleted && c.Id == pl.ContractId && c.Active)))
                .Select(g => g.Key)
                .ToListAsync();

            if (estToActivate.Count == 0) return 0;

            return await _context.Set<Establishment>()
                .Where(e => estToActivate.Contains(e.Id) && !e.Active)
                .ExecuteUpdateAsync(s => s.SetProperty(e => e.Active, true));
        }
    }
}
