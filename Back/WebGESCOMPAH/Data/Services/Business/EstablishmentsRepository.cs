using Data.Interfaz.IDataImplement.Business;
using Data.Repository;
using Entity.Domain.Models.Implements.Business;
using Entity.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using Entity.DTOs.Implements.Business.EstablishmentDto;
using Entity.Enum;

namespace Data.Services.Business
{
    /// <summary>
    /// Repositorio para manejar operaciones relacionadas con los establecimientos (Establishment).
    /// Incluye consultas optimizadas con filtros, proyecciones y actualizaciones masivas.
    /// </summary>
    public class EstablishmentsRepository : DataGeneric<Establishment>, IEstablishmentsRepository
    {
        public EstablishmentsRepository(ApplicationDbContext context) : base(context) { }

        // ================== QUERIES BASE ==================

        /// <summary>
        /// Construye la query base para establecimientos aplicando filtros comunes:
        /// - Excluye eliminados (IsDeleted).
        /// - Incluye solo plazas activas y no eliminadas.
        /// - Incluye la plaza asociada.
        /// - Si includeImages = true, incluye solo la primera imagen activa (ordenada por Id).
        /// - Ordena resultados por fecha de creaci?n y luego por Id en orden descendente.
        /// </summary>
        private IQueryable<Establishment> BaseQuery(bool includeImages = true)
        {
            var query = _dbSet.AsNoTracking()
                .Where(e => !e.IsDeleted &&
                            e.Plaza != null && e.Plaza.Active && !e.Plaza.IsDeleted);

            if (includeImages)
            {
                query = query
                    .Include(e => e.Plaza)
                    .Include(e => e.Images
                        .Where(img => img.Active && !img.IsDeleted)
                        .OrderBy(img => img.Id)
                        .Take(1)); // Solo toma la primera imagen activa para optimizar rendimiento
            }
            else
            {
                query = query.Include(e => e.Plaza);
            }

            return query
                .OrderByDescending(e => e.CreatedAt)
                .ThenByDescending(e => e.Id);
        }

        /// <summary>
        /// Verifica si la coleccion de IDs es nula o esta vacia.
        /// </summary>
        private static bool IsEmpty(IReadOnlyCollection<int> ids) =>
            ids == null || ids.Count == 0;

        // ================== METODOS ==================

        /// <summary>
        /// Obtiene todos los establecimientos (compatibilidad).
        /// </summary>
        public override Task<IEnumerable<Establishment>> GetAllAsync() =>
            GetAllAsync(ActivityFilter.Any);

        /// <summary>
        /// Obtiene todos los establecimientos filtrados por actividad.
        /// </summary>
        /// <param name="filter">Filtro de actividad (Any / ActiveOnly).</param>
        public async Task<IEnumerable<Establishment>> GetAllAsync(ActivityFilter filter, int? limit = null)
        {
            var q = BaseQuery(includeImages: true);
            if (filter == ActivityFilter.ActiveOnly)
                q = q.Where(e => e.Active);

            if (limit.HasValue && limit.Value > 0)
                q = q.Take(limit.Value);

            return await q.ToListAsync();
        }

        public async Task<IEnumerable<Establishment>> GetByPlazaIdAsync(int plazaId, ActivityFilter filter, int? limit = null)
        {
            var q = BaseQuery(includeImages: true)
                .Where(e => e.PlazaId == plazaId);

            if (filter == ActivityFilter.ActiveOnly)
                q = q.Where(e => e.Active);

            if (limit.HasValue && limit.Value > 0)
                q = q.Take(limit.Value);

            return await q.ToListAsync();
        }

        /// <summary>
        /// Obtiene un establecimiento por Id (incluyendo eliminados logicamente inactivos).
        /// </summary>
        public Task<Establishment?> GetByIdAnyAsync(int id) =>
            BaseQuery().FirstOrDefaultAsync(e => e.Id == id);

        /// <summary>
        /// Obtiene un establecimiento por Id unicamente si esta activo.
        /// </summary>
        public Task<Establishment?> GetByIdActiveAsync(int id) =>
            BaseQuery().Where(e => e.Active).FirstOrDefaultAsync(e => e.Id == id);

        /// <summary>
        /// Obtiene solo informacion basica de establecimientos (Id, RentValueBase, UvtQty).
        /// </summary>
        public async Task<IReadOnlyList<EstablishmentBasicsDto>> GetBasicsByIdsAsync(IReadOnlyCollection<int> ids)
        {
            if (IsEmpty(ids)) return Array.Empty<EstablishmentBasicsDto>();

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

        /// <summary>
        /// Obtiene una lista liviana de establecimientos para mostrar en tarjetas.
        /// Incluye solo una imagen principal (la primera activa).
        /// </summary>
        public async Task<IReadOnlyList<EstablishmentCardDto>> GetCardsAsync(ActivityFilter filter)
        {
            var q = BaseQuery(includeImages: false);

            if (filter == ActivityFilter.ActiveOnly)
                q = q.Where(e => e.Active);

            return await q
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

        /// <summary>
        /// Devuelve los Ids de los establecimientos inactivos dentro de una lista de Ids.
        /// </summary>
        public async Task<IReadOnlyList<int>> GetInactiveIdsAsync(IReadOnlyCollection<int> ids)
        {
            if (IsEmpty(ids)) return Array.Empty<int>();

            return await _dbSet.AsNoTracking()
                .Where(e => ids.Contains(e.Id) && !e.IsDeleted && !e.Active)
                .Select(e => e.Id)
                .ToListAsync();
        }

        /// <summary>
        /// Activa o desactiva en masa establecimientos segun una lista de Ids.
        /// </summary>
        /// <param name="ids">Lista de Ids a actualizar.</param>
        /// <param name="active">Nuevo estado (true = activo, false = inactivo).</param>
        public async Task<int> SetActiveByIdsAsync(IReadOnlyCollection<int> ids, bool active)
        {
            if (IsEmpty(ids)) return 0;

            return await _dbSet
                .Where(e => ids.Contains(e.Id) && !e.IsDeleted && e.Active != active)
                .ExecuteUpdateAsync(up => up.SetProperty(e => e.Active, _ => active));
        }

        /// <summary>
        /// Activa o desactiva en masa todos los establecimientos de una plaza especifica.
        /// </summary>
        /// <param name="plazaId">Id de la plaza.</param>
        /// <param name="active">Nuevo estado (true = activo, false = inactivo).</param>
        public async Task<int> SetActiveByPlazaIdAsync(int plazaId, bool active) =>
            await _dbSet
                .Where(e => e.PlazaId == plazaId && !e.IsDeleted && e.Active != active)
                .ExecuteUpdateAsync(up => up.SetProperty(e => e.Active, _ => active));
    }
}
