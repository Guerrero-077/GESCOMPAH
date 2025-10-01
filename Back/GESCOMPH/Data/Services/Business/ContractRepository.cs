using Data.Interfaz.IDataImplement.Business;
using Data.Repository;
using Entity.Domain.Models.Implements.Business;
using Entity.DTOs.Implements.Business.Contract;
using Entity.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using Utilities.Exceptions;

namespace Data.Services.Business
{
    public class ContractRepository : DataGeneric<Contract>, IContractRepository
    {
        public ContractRepository(ApplicationDbContext context) : base(context) { }

        // ================== QUERIES BASE ==================

        private IQueryable<Contract> GetContractFullQuery()
        {
            return _dbSet
                .Include(c => c.Person).ThenInclude(p => p.User)
                .Include(c => c.PremisesLeased).ThenInclude(pl => pl.Establishment).ThenInclude(e => e.Plaza)
                .Include(c => c.ContractClauses).ThenInclude(cc => cc.Clause)
                .AsNoTracking();
        }

        /// <summary>
        /// Query base para tarjetas de contrato, con filtro opcional por persona.
        /// </summary>
        private IQueryable<ContractCardDto> GetCardQuery(int? personId = null)
        {
            var query = _dbSet.AsNoTracking()
                .Where(c => !c.IsDeleted);

            if (personId.HasValue)
                query = query.Where(c => c.PersonId == personId.Value);

            return query
                .OrderByDescending(e => e.Active)
                .ThenByDescending(e => e.CreatedAt)
                .ThenByDescending(e => e.Id)
                .Select(c => new ContractCardDto(
                    c.Id,
                    c.PersonId,
                    c.Person.FirstName,
                    c.Person.LastName,
                    c.Person.Document,
                    c.Person.Phone,
                    c.Person.User != null ? c.Person.User.Email : null,
                    c.StartDate,
                    c.EndDate,
                    c.TotalBaseRentAgreed,
                    c.TotalUvtQtyAgreed,
                    c.Active
                ));
        }

        // ================== MÉTODOS ==================

        public override async Task<Contract?> GetByIdAsync(int id) =>
            await GetContractFullQuery().FirstOrDefaultAsync(c => c.Id == id);

        public async Task<IReadOnlyList<ContractCardDto>> GetCardsByPersonAsync(int personId) =>
            await GetCardQuery(personId).ToListAsync();

        public async Task<IReadOnlyList<ContractCardDto>> GetCardsAllAsync() =>
            await GetCardQuery().ToListAsync();

        /// <summary>
        /// Crea un contrato con sus locales y cláusulas.
        /// Calcula automáticamente los totales a partir de los establecimientos.
        /// </summary>
        public async Task<int> CreateContractAsync(ContractCreateDto dto, int personId, IReadOnlyCollection<int> establishmentIds)
        {
            if (establishmentIds == null || establishmentIds.Count == 0)
                throw new BusinessException("Debe seleccionar al menos un establecimiento.");

            // Totales
            var basics = await _context.Set<Establishment>()
                .AsNoTracking()
                .Where(e => establishmentIds.Contains(e.Id))
                .Select(e => new { e.RentValueBase, e.UvtQty })
                .ToListAsync();

            var totalBase = basics.Sum(b => b.RentValueBase);
            var totalUvt = basics.Sum(b => b.UvtQty);

            // Contrato
            var contract = new Contract
            {
                PersonId = personId,
                TotalBaseRentAgreed = totalBase,
                TotalUvtQtyAgreed = totalUvt,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                Active = true // o según regla de negocio
            };

            foreach (var estId in establishmentIds)
                contract.PremisesLeased.Add(new PremisesLeased { EstablishmentId = estId });

            await _dbSet.AddAsync(contract);

            // Cláusulas
            if (dto.ClauseIds is { Count: > 0 })
            {
                var uniqueClauseIds = dto.ClauseIds.Distinct().ToList();
                var links = uniqueClauseIds.Select(cid => new ContractClause
                {
                    Contract = contract,
                    ClauseId = cid
                });
                await _context.contractClauses.AddRangeAsync(links);
            }

            // Desactivar establecimientos
            await _context.Set<Establishment>()
                .Where(e => establishmentIds.Contains(e.Id))
                .ExecuteUpdateAsync(up => up.SetProperty(e => e.Active, _ => false));

            await _context.SaveChangesAsync();
            return contract.Id;
        }

        /// <summary>
        /// Desactiva contratos con EndDate < utcNow y que sigan activos.
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
        /// Libera establecimientos de contratos expirados si no están ocupados por otro contrato activo.
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

            // Establecimientos sin otro contrato activo que los mantenga ocupados
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

        /// <summary>
        /// Indica si existen contratos activos (no eliminados) que tengan locales de la plaza indicada.
        /// </summary>
        public async Task<bool> AnyActiveByPlazaAsync(int plazaId)
        {
            return await _dbSet.AsNoTracking()
                .Where(c => !c.IsDeleted && c.Active)
                .SelectMany(c => c.PremisesLeased)
                .AnyAsync(pl => !pl.IsDeleted &&
                                !pl.Establishment.IsDeleted &&
                                pl.Establishment.PlazaId == plazaId);
        }
    }
}
