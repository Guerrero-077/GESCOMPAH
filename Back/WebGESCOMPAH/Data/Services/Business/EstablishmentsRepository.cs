using Data.Interfaz.IDataImplement.Business;
using Data.Repository;
using Entity.Domain.Models.Implements.Business;
using Entity.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using Entity.DTOs.Implements.Business.EstablishmentDto;
using Entity.Enum;

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
        public async Task<IReadOnlyList<EstablishmentBasicsDto>> GetBasicsByIdsAsync(IReadOnlyCollection<int> ids)
        {
            if (ids.Count == 0) return Array.Empty<EstablishmentBasicsDto>();
            return await _dbSet.AsNoTracking()
                .Where(e => ids.Contains(e.Id) && e.Active && !e.IsDeleted)
                .Select(e => new EstablishmentBasicsDto
                {
                    Id = e.Id,
                    RentValueBase = e.RentValueBase,
                    UvtQty = e.UvtQty
                })
                .ToListAsync();
        }

        // Lista liviana para tarjetas
        public async Task<IReadOnlyList<EstablishmentCardDto>> GetCardsAsync(ActivityFilter filter)
        {
            var q = _dbSet.AsNoTracking()
                .Where(e => !e.IsDeleted && e.Plaza != null && e.Plaza.Active && !e.Plaza.IsDeleted);

            if (filter == ActivityFilter.ActiveOnly)
                q = q.Where(e => e.Active);

            return await q
                .OrderByDescending(e => e.CreatedAt)
                .ThenByDescending(e => e.Id)
                .Select(e => new EstablishmentCardDto
                {
                    Id = e.Id,
                    Name = e.Name,
                    Description = e.Description,
                    Address = e.Address,
                    AreaM2 = e.AreaM2,
                    RentValueBase = e.RentValueBase,
                    Active = e.Active,
                    PrimaryImagePath = e.Images
                        .Where(img => img.Active && !img.IsDeleted)
                        .OrderBy(img => img.Id)
                        .Select(img => img.FilePath)
                        .FirstOrDefault()
                })
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

        // Actualización masiva por PlazaId
        public async Task<int> SetActiveByPlazaIdAsync(int plazaId, bool active) =>
            await _dbSet
                .Where(e => e.PlazaId == plazaId && !e.IsDeleted && e.Active != active)
                .ExecuteUpdateAsync(up => up.SetProperty(e => e.Active, _ => active));
    }
}
