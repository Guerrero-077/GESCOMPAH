using Entity.Domain.Models.ModelBase;
using Entity.DTOs.Base;
using Entity.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Utilities.Exceptions;

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
        /// - Orden estable: CreatedAt DESC, Id DESC ( til para paginaci n determinista).
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
                throw new EntityNotFoundException(typeof(T).Name, entity.Id);

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

        #region Query din mica con paginado

        public override async Task<PagedResult<T>> QueryAsync(
            PageQuery query,
            Expression<Func<T, string>>[]? searchableExpressions = null,
            Expression<Func<T, bool>>[]? extraFilters = null,
            IDictionary<string, LambdaExpression>? sortMap = null)
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

            // 3) B squeda libre (OR-Like) sobre campos string (case-insensitive)
            if (!string.IsNullOrWhiteSpace(query.Search) &&
                searchableExpressions is { Length: > 0 })
            {
                var raw = query.Search.Trim();
                var text = raw.ToLower(); // normaliza a min sculas
                // Usar LIKE con el escape por defecto de MySQL (\\) sin pasar el 3er par metro (algunos providers no lo soportan)
                var pattern = $"%{EscapeLike(text, '\\')}%";

                var toLower = typeof(string).GetMethod(nameof(string.ToLower), Type.EmptyTypes)!;
                Expression? orBody = null;
                var param = Expression.Parameter(typeof(T), "e");

                foreach (var expr in searchableExpressions)
                {
                    // Coalesce para evitar null y normalizar a min sculas
                    var coalescedExpr = Expression.Coalesce(
                        expr.Body,
                        Expression.Constant(string.Empty)
                    );
                    var lowered = Expression.Call(coalescedExpr, toLower);

                    // EF.Functions.Like(lowered, pattern)
                    var likeMethod = typeof(DbFunctionsExtensions)
                        .GetMethods()
                        .First(m => m.Name == nameof(DbFunctionsExtensions.Like)
                                    && m.GetParameters().Length == 3); // (DbFunctions, string, string)

                    var likeCall = Expression.Call(
                        likeMethod!,
                        Expression.Constant(EF.Functions),
                        lowered,
                        Expression.Constant(pattern)
                    );

                    var fieldLambda = Expression.Lambda<Func<T, bool>>(likeCall, expr.Parameters);
                    var replaced = new ParameterReplaceVisitor(fieldLambda.Parameters[0], param)
                        .Visit(fieldLambda.Body)!;
                    orBody = orBody == null ? replaced : Expression.OrElse(orBody, replaced);
                }

                if (orBody != null)
                {
                    var finalLambda = Expression.Lambda<Func<T, bool>>(orBody, param);
                    q = q.Where(finalLambda);
                }
            }

            // 4) Orden dinámico (prioriza sortMap si fue provisto)
            if (!string.IsNullOrWhiteSpace(query.Sort) && sortMap != null && sortMap.TryGetValue(query.Sort, out var selector))
            {
                var keyType = selector.ReturnType;
                var orderByName = query.Desc ? nameof(Queryable.OrderByDescending) : nameof(Queryable.OrderBy);
                var thenByName = query.Desc ? nameof(Queryable.ThenByDescending) : nameof(Queryable.ThenBy);

                var orderBy = typeof(Queryable).GetMethods()
                    .Where(m => m.Name == orderByName && m.GetParameters().Length == 2)
                    .Single()
                    .MakeGenericMethod(typeof(T), keyType);

                var thenBy = typeof(Queryable).GetMethods()
                    .Where(m => m.Name == thenByName && m.GetParameters().Length == 2)
                    .Single()
                    .MakeGenericMethod(typeof(T), typeof(int));

                q = (IQueryable<T>)orderBy.Invoke(null, new object[] { q, Expression.Quote(selector) })!;

                var param = Expression.Parameter(typeof(T), "e");
                var idProp = Expression.Property(param, nameof(BaseModel.Id));
                var idSelector = Expression.Lambda(idProp, param);
                q = (IQueryable<T>)thenBy.Invoke(null, new object[] { q, Expression.Quote(idSelector) })!;
            }
            else
            {
                q = ApplySorting(q, query.Sort, query.Desc);
            }

            // 5) Paginaci n
            var total = await q.CountAsync();
            var items = await q.Skip((page - 1) * size)
                               .Take(size)
                               .ToListAsync();

            return new PagedResult<T>(items, total, page, size);
        }

        #endregion

        #region Helpers

        private sealed class ParameterReplaceVisitor : ExpressionVisitor
        {
            private readonly ParameterExpression _from;
            private readonly ParameterExpression _to;
            public ParameterReplaceVisitor(ParameterExpression from, ParameterExpression to)
            { _from = from; _to = to; }
            protected override Expression VisitParameter(ParameterExpression node)
                => node == _from ? _to : base.VisitParameter(node);
        }

        private static IQueryable<T> ApplySorting(IQueryable<T> q, string? sort, bool desc)
        {
            if (string.IsNullOrWhiteSpace(sort))
                return q; // mantiene orden base

            var param = Expression.Parameter(typeof(T), "e");
            var body = BuildPropertyAccess(param, sort);
            if (body is null)
                return q; // sort desconocido → mantener orden base

            var keyType = body.Type;
            var keySelector = Expression.Lambda(body, param);

            var orderByName = desc ? nameof(Queryable.OrderByDescending) : nameof(Queryable.OrderBy);
            var thenByName = desc ? nameof(Queryable.ThenByDescending) : nameof(Queryable.ThenBy);

            var orderBy = typeof(Queryable).GetMethods()
                .Where(m => m.Name == orderByName && m.GetParameters().Length == 2)
                .Single()
                .MakeGenericMethod(typeof(T), keyType);

            var thenBy = typeof(Queryable).GetMethods()
                .Where(m => m.Name == thenByName && m.GetParameters().Length == 2)
                .Single()
                .MakeGenericMethod(typeof(T), typeof(int));

            var ordered = (IQueryable<T>)orderBy.Invoke(null, new object[] { q, Expression.Quote(keySelector) })!;

            // desempate por Id para estabilidad
            var idProp = Expression.Property(param, nameof(BaseModel.Id));
            var idSelector = Expression.Lambda(idProp, param);
            ordered = (IQueryable<T>)thenBy.Invoke(null, new object[] { ordered, Expression.Quote(idSelector) })!;

            return ordered;
        }

        private static Expression? BuildPropertyAccess(ParameterExpression param, string path)
        {
            // Soporta rutas anidadas: Ej. "Person.Document", "Person.City.Name"
            Expression current = param;
            foreach (var segment in path.Split('.', StringSplitOptions.RemoveEmptyEntries))
            {
                var prop = current.Type.GetProperty(segment);
                if (prop == null) return null;
                current = Expression.Property(current, prop);
            }
            return current;
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



