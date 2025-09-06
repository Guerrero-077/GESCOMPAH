using Data.Interfaz.IDataImplement.Business;
using Data.Repository;
using Entity.Domain.Models.Implements.Business;
using Entity.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Data.Services.Business
{
    public class EstablishmentsRepository : DataGeneric<Establishment>, IEstablishmentsRepository
    {
        public EstablishmentsRepository(ApplicationDbContext context) : base(context) { }

        // Query base: filtros comunes
        private IQueryable<Establishment> BaseQuery() =>
            _dbSet.AsNoTracking()
                .OrderByDescending(e => e.CreatedAt)
                .ThenByDescending(e => e.Id)
                  .Where(e => !e.IsDeleted &&
                              e.Plaza != null && e.Plaza.Active && !e.Plaza.IsDeleted)
                  .Include(e => e.Plaza)
                  .Include(e => e.Images.Where(img => img.Active && !img.IsDeleted));

        // Compatibilidad: trae todos
        public override Task<IEnumerable<Establishment>> GetAllAsync() =>
            GetAllAsync(ActivityFilter.Any);

        // Sobrecarga
        public async Task<IEnumerable<Establishment>> GetAllAsync(ActivityFilter filter)
        {
            var q = BaseQuery();
            if (filter == ActivityFilter.ActiveOnly)
                q = q.Where(e => e.Active);

            return await q.ToListAsync();
        }

        // Detalle
        public Task<Establishment?> GetByIdAnyAsync(int id) =>
            BaseQuery().FirstOrDefaultAsync(e => e.Id == id);

        public Task<Establishment?> GetByIdActiveAsync(int id) =>
            BaseQuery().Where(e => e.Active).FirstOrDefaultAsync(e => e.Id == id);

        // Proyección
        public async Task<IReadOnlyList<EstablishmentBasics>> GetBasicsByIdsAsync(IReadOnlyCollection<int> ids)
        {
            if (ids.Count == 0) return Array.Empty<EstablishmentBasics>();
            return await _dbSet.AsNoTracking()
                .Where(e => ids.Contains(e.Id) && e.Active && !e.IsDeleted)
                .Select(e => new EstablishmentBasics(e.Id, e.RentValueBase, e.UvtQty))
                .ToListAsync();
        }

        // Validación
        public async Task<IReadOnlyList<int>> GetInactiveIdsAsync(IReadOnlyCollection<int> ids) =>
            ids.Count == 0
                ? Array.Empty<int>()
                : await _dbSet.AsNoTracking()
                    .Where(e => ids.Contains(e.Id) && !e.IsDeleted && !e.Active)
                    .Select(e => e.Id)
                    .ToListAsync();

        // Actualización masiva
        public async Task<int> SetActiveByIdsAsync(IReadOnlyCollection<int> ids, bool active) =>
            ids.Count == 0
                ? 0
                : await _dbSet
                    .Where(e => ids.Contains(e.Id) && !e.IsDeleted && e.Active != active)
                    .ExecuteUpdateAsync(up => up.SetProperty(e => e.Active, _ => active));
    }
}
