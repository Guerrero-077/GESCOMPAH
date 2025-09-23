using Entity.Domain.Models.ModelBase;
using Entity.DTOs.Base;
using Entity.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Reflection;

namespace Data.Repository
{
    public class DataGeneric<T> : ADataGenerica<T> where T : BaseModel
    {
        // DbContext de la aplicación.
        protected readonly ApplicationDbContext _context;
        // DbSet tipado de la entidad.
        protected readonly DbSet<T> _dbSet;

        public DataGeneric(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        /// <summary>
        /// Query base:
        ///  - AsNoTracking para lecturas (evita overhead del change-tracker).
        ///  - Filtra soft-delete (!IsDeleted).
        ///  - Orden base estable por CreatedAt DESC, Id DESC (garantiza determinismo en paginado).
        /// </summary>
        private IQueryable<T> BaseQuery() =>
            _dbSet.AsNoTracking()
                  .Where(e => !e.IsDeleted)
                  .OrderByDescending(e => e.CreatedAt)
                  .ThenByDescending(e => e.Id);

        // ==========================
        // ========== CRUD ==========
        // ==========================

        public override async Task<IEnumerable<T>> GetAllAsync()
            => await BaseQuery().ToListAsync();

        public override async Task<T?> GetByIdAsync(int id)
            => await _dbSet.AsNoTracking()
                           .FirstOrDefaultAsync(e => e.Id == id && !e.IsDeleted);

        public override async Task<T> AddAsync(T entity)
        {
            // Alta de entidad.
            _dbSet.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public override async Task<T> UpdateAsync(T entity)
        {
            // Buscamos la entidad viva (no borrada lógicamente).
            var existing = await _dbSet.FirstOrDefaultAsync(
                x => x.Id == entity.Id && !x.IsDeleted);

            if (existing is null)
                throw new InvalidOperationException($"No se encontró entidad con ID {entity.Id}");

            // Protegemos campos inmutables.
            var originalCreatedAt = existing.CreatedAt;
            var originalId = existing.Id;

            // Copiamos valores desde el DTO entrante.
            _context.Entry(existing).CurrentValues.SetValues(entity);

            // Reforzamos invariantes.
            existing.Id = originalId;
            existing.CreatedAt = originalCreatedAt;

            await _context.SaveChangesAsync();
            return existing;
        }

        public override async Task<bool> DeleteAsync(int id)
        {
            // Baja física (solo si tu dominio lo permite).
            var entity = await _dbSet.FindAsync(id);
            if (entity == null) return false;
            _dbSet.Remove(entity);
            return await _context.SaveChangesAsync() > 0;
        }

        public override async Task<bool> DeleteLogicAsync(int id)
        {
            // Soft delete (IsDeleted = true).
            var entity = await _dbSet.FindAsync(id);
            if (entity == null) return false;

            entity.IsDeleted = true;
            _dbSet.Update(entity);
            return await _context.SaveChangesAsync() > 0;
        }

        /// <summary>
        /// Exposición de la query base (read-only) para usos avanzados.
        /// </summary>
        public override IQueryable<T> GetAllQueryable() => BaseQuery();

        // ===============================================
        // ======== Query genérica con paginado ==========
        // ===============================================

        public override async Task<PagedResult<T>> QueryAsync(
            PageQuery query,
            Expression<Func<T, string>>[]? searchableExpressions = null,
            Expression<Func<T, bool>>[]? extraFilters = null)
        {
            // 0) Saneamos Page/Size (defensa server-side).
            var page = query.Page <= 0 ? 1 : query.Page;
            var size = query.Size <= 0 ? 20 : Math.Min(query.Size, 200); // cota de seguridad

            // 1) Partimos de la query base (ya trae AsNoTracking + !IsDeleted + orden estable).
            IQueryable<T> q = GetAllQueryable();

            // 2) Filtros tipados adicionales (los define/valida Business para evitar inyección).
            if (extraFilters is not null)
            {
                foreach (var f in extraFilters)
                {
                    q = q.Where(f);
                }
            }

            // 3) Búsqueda libre (OR-Like) sobre campos string permitidos.
            if (!string.IsNullOrWhiteSpace(query.Search) &&
                searchableExpressions is { Length: > 0 })
            {
                var text = query.Search.Trim();

                // Construimos "%texto%" con escape.
                const char escape = '\\';
                var pattern = $"%{EscapeLike(text, escape)}%";

                // Construimos una expresión: e => Like(Coalesce(campo, ""), pattern, escape) OR ...
                Expression? orChain = null;
                var param = Expression.Parameter(typeof(T), "e");

                // EF.Functions (DbFunctions) - propiedad estática: EF.Functions
                var efFunctionsProp = typeof(EF).GetProperty(nameof(EF.Functions),
                                         BindingFlags.Public | BindingFlags.Static)
                                     ?? throw new InvalidOperationException("No se encontró EF.Functions.");
                var efFunctionsExpr = Expression.Property(null, efFunctionsProp);

                // Seleccionamos el overload Like(DbFunctions, string, string, char)
                var likeMethod = typeof(DbFunctionsExtensions).GetMethods(BindingFlags.Public | BindingFlags.Static)
                    .FirstOrDefault(m =>
                        m.Name == nameof(DbFunctionsExtensions.Like) &&
                        m.GetParameters().Length == 4 &&
                        m.GetParameters()[0].ParameterType == typeof(DbFunctions))
                    ?? throw new InvalidOperationException("No se encontró EF.Functions.Like(DbFunctions, string, string, char).");

                foreach (var expr in searchableExpressions)
                {
                    // Reemplazamos el parámetro del campo buscable por 'e'.
                    var replaced = ParameterReplacer.Replace(expr.Body, expr.Parameters[0], param);

                    // Coalesce a string.Empty para evitar null en LIKE.
                    var coalesced = Expression.Coalesce(replaced, Expression.Constant(string.Empty));

                    // Llamada: EF.Functions.Like(coalesced, pattern, escape).
                    var call = Expression.Call(
                        likeMethod,
                        efFunctionsExpr,
                        coalesced,
                        Expression.Constant(pattern),
                        Expression.Constant(escape));

                    // Acumulamos con OR.
                    orChain = orChain is null ? call : Expression.OrElse(orChain, call);
                }

                if (orChain is not null)
                {
                    var lambda = Expression.Lambda<Func<T, bool>>(orChain, param);
                    q = q.Where(lambda);
                }
            }

            // 4) Ordenamiento dinámico y seguro.
            //    - Si no viene Sort, se respeta el orden de BaseQuery (CreatedAt DESC, Id DESC).
            //    - Business debe whitelistear 'Sort' para evitar abuso.
            q = ApplySorting(q, query.Sort, query.Desc);

            // 5) Total + items paginados
            var total = await q.CountAsync();
            var items = await q.Skip((page - 1) * size)
                               .Take(size)
                               .ToListAsync();

            return new PagedResult<T>(items, total, page, size);
        }

        /// <summary>
        /// Ordena por nombre de propiedad usando EF.Property (dinámico), con fallback al orden estable.
        /// IMPORTANTE: Business debe validar que 'sort' esté en la lista blanca para evitar abuso.
        /// </summary>
        private static IQueryable<T> ApplySorting(IQueryable<T> q, string? sort, bool desc)
        {
            if (string.IsNullOrWhiteSpace(sort))
                return q; // Se mantiene el orden de BaseQuery.

            // e => EF.Property<object>(e, sort)
            var param = Expression.Parameter(typeof(T), "e");
            var body = Expression.Call(
                typeof(EF).GetMethod(nameof(EF.Property))!.MakeGenericMethod(typeof(object)),
                param,
                Expression.Constant(sort));

            var lambda = Expression.Lambda<Func<T, object>>(
                Expression.Convert(body, typeof(object)), param);

            // Aplicamos orden y reforzamos estabilidad con ThenBy(Id).
            return desc
                ? q.OrderByDescending(lambda).ThenByDescending(e => e.Id)
                : q.OrderBy(lambda).ThenBy(e => e.Id);
        }

        /// <summary>
        /// Escapa un texto para usarlo en un patrón SQL LIKE con un caracter de escape dado.
        /// Reemplaza el propio 'escape', luego '%' y '_' para que se busquen literal.
        /// Ej.: con escape '\', "50%_OFF" => "50\%\_OFF"
        /// </summary>
        private static string EscapeLike(string input, char escapeChar)
        {
            return input
                .Replace(escapeChar.ToString(), $"{escapeChar}{escapeChar}")
                .Replace("%", $"{escapeChar}%")
                .Replace("_", $"{escapeChar}_");
        }
    }
}
