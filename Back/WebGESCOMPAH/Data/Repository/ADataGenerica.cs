using Entity.Domain.Models.ModelBase;
using Entity.DTOs.Base;
using System.Linq.Expressions;

namespace Data.Repository
{
    public abstract class ADataGenerica<T> : Data.Interfaz.DataBasic.IDataGeneric<T> where T : BaseModel
    {
        public abstract Task<IEnumerable<T>> GetAllAsync();
        public abstract Task<T?> GetByIdAsync(int id);
        public abstract Task<T> AddAsync(T entity);
        public abstract Task<T> UpdateAsync(T entity);
        public abstract Task<bool> DeleteAsync(int id);
        public abstract Task<bool> DeleteLogicAsync(int id);

        // Retorna la query base (read-only); útil para pipelines custom.
        public abstract IQueryable<T> GetAllQueryable();

        /// <summary>
        /// Consulta genérica (server-side):
        ///  - Aplica filtros tipados (provenientes de Business, ya validados).
        ///  - Búsqueda OR sobre campos string permitidos, usando EF.Functions.Like (index-friendly).
        ///  - Ordenamiento dinámico seguro (EF.Property).
        ///  - Paginado con cota de seguridad.
        /// </summary>
        public abstract Task<PagedResult<T>> QueryAsync(
            PageQuery query,
            Expression<Func<T, string>>[]? searchableExpressions = null,
            Expression<Func<T, bool>>[]? extraFilters = null,
            IDictionary<string, LambdaExpression>? sortMap = null);
    }
}
