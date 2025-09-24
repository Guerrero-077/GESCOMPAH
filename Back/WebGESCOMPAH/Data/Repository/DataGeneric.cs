using Entity.Domain.Models.ModelBase;
using Entity.DTOs.Base;
using Entity.Infrastructure.Context;
using LinqKit; // necesario para PredicateBuilder y AsExpandable
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Reflection;

namespace Data.Repository
{
    public class DataGeneric<T> : ADataGenerica<T> where T : BaseModel
    {
        protected readonly ApplicationDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public DataGeneric(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        /// <summary>
        /// Query base:
        /// - AsNoTracking para lecturas (evita overhead del change-tracker).
        /// - Filtra soft-delete (!IsDeleted).
        /// - Orden estable: CreatedAt DESC, Id DESC (útil para paginación determinista).
        /// </summary>
        private IQueryable<T> BaseQuery() =>
            _dbSet.AsNoTracking()
                  .Where(e => !e.IsDeleted)
                  .OrderByDescending(e => e.CreatedAt)
                  .ThenByDescending(e => e.Id);

        #region CRUD

        public override async Task<IEnumerable<T>> GetAllAsync()
            => await BaseQuery().ToListAsync();

        public override async Task<T?> GetByIdAsync(int id)
            => await _dbSet.AsNoTracking()
                           .FirstOrDefaultAsync(e => e.Id == id && !e.IsDeleted);

        public override async Task<T> AddAsync(T entity)
        {
            _dbSet.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public override async Task<T> UpdateAsync(T entity)
        {
            var existing = await _dbSet.FirstOrDefaultAsync(
                x => x.Id == entity.Id && !x.IsDeleted);

            if (existing is null)
                throw new InvalidOperationException($"No se encontró entidad con ID {entity.Id}");

            // Campos inmutables
            var originalCreatedAt = existing.CreatedAt;
            var originalId = existing.Id;

            _context.Entry(existing).CurrentValues.SetValues(entity);

            existing.Id = originalId;
            existing.CreatedAt = originalCreatedAt;

            await _context.SaveChangesAsync();
            return existing;
        }

        public override async Task<bool> DeleteAsync(int id)
        {
            var entity = await _dbSet.FindAsync(id);
            if (entity == null) return false;

            _dbSet.Remove(entity);
            return await _context.SaveChangesAsync() > 0;
        }

        public override async Task<bool> DeleteLogicAsync(int id)
        {
            var entity = await _dbSet.FindAsync(id);
            if (entity == null) return false;

            entity.IsDeleted = true;
            _dbSet.Update(entity);
            return await _context.SaveChangesAsync() > 0;
        }

        public override IQueryable<T> GetAllQueryable() => BaseQuery();

        #endregion

        #region Query dinámica con paginado

        public override async Task<PagedResult<T>> QueryAsync(
            PageQuery query,
            Expression<Func<T, string>>[]? searchableExpressions = null,
            Expression<Func<T, bool>>[]? extraFilters = null)
        {
            // 0) Saneamos page y size
            var page = query.Page <= 0 ? 1 : query.Page;
            var size = query.Size <= 0 ? 20 : Math.Min(query.Size, 200);

            // 1) Partimos de la query base
            IQueryable<T> q = GetAllQueryable();

            // 2) Aplicamos filtros extra
            if (extraFilters != null)
            {
                foreach (var f in extraFilters)
                    q = q.Where(f);
            }

            // 3) Búsqueda libre (OR-Like) sobre campos string
            if (!string.IsNullOrWhiteSpace(query.Search) &&
                searchableExpressions is { Length: > 0 })
            {
                var text = query.Search.Trim();
                const string escape = "\\"; // EF.Functions.Like espera string, no char
                var pattern = $"%{EscapeLike(text, escape[0])}%";

                var predicate = PredicateBuilder.New<T>(false); // inicia en false (OR)

                foreach (var expr in searchableExpressions)
                {
                    // Coalesce para evitar null
                    var coalescedExpr = Expression.Coalesce(
                        expr.Body,
                        Expression.Constant(string.Empty)
                    );

                    // Construimos EF.Functions.Like como Expression.Call (traducible a SQL)
                    var likeCall = Expression.Call(
                        typeof(DbFunctionsExtensions),
                        nameof(DbFunctionsExtensions.Like),
                        Type.EmptyTypes,
                        Expression.Constant(EF.Functions),
                        coalescedExpr,
                        Expression.Constant(pattern),
                        Expression.Constant(escape)
                    );

                    var lambda = Expression.Lambda<Func<T, bool>>(likeCall, expr.Parameters);

                    predicate = predicate.Or(lambda);
                }

                q = q.AsExpandable().Where(predicate);
            }

            // 4) Orden dinámico
            q = ApplySorting(q, query.Sort, query.Desc);

            // 5) Paginación
            var total = await q.CountAsync();
            var items = await q.Skip((page - 1) * size)
                               .Take(size)
                               .ToListAsync();

            return new PagedResult<T>(items, total, page, size);
        }

        #endregion

        #region Helpers

        private static IQueryable<T> ApplySorting(IQueryable<T> q, string? sort, bool desc)
        {
            if (string.IsNullOrWhiteSpace(sort))
                return q; // mantiene orden base

            var param = Expression.Parameter(typeof(T), "e");
            var body = Expression.Call(
                typeof(EF).GetMethod(nameof(EF.Property))!.MakeGenericMethod(typeof(object)),
                param,
                Expression.Constant(sort)
            );

            var lambda = Expression.Lambda<Func<T, object>>(
                Expression.Convert(body, typeof(object)), param
            );

            return desc
                ? q.OrderByDescending(lambda).ThenByDescending(e => e.Id)
                : q.OrderBy(lambda).ThenBy(e => e.Id);
        }

        private static string EscapeLike(string input, char escapeChar)
        {
            return input
                .Replace(escapeChar.ToString(), $"{escapeChar}{escapeChar}")
                .Replace("%", $"{escapeChar}%")
                .Replace("_", $"{escapeChar}_");
        }

        #endregion
    }
}
