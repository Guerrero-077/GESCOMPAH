using Entity.DTOs.Base;
using System.Linq.Expressions;

namespace Data.Interfaz.DataBasic
{
    public interface IDataGeneric<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<T?> GetByIdAsync(int id);
        Task<T> AddAsync(T entity);
        Task<T> UpdateAsync(T entity);

        Task<bool> DeleteAsync(int id);

        Task<bool> DeleteLogicAsync(int id);

        IQueryable<T> GetAllQueryable();

        // Consulta paginada + búsqueda + sort, con filtros tipados ya validados en Business.
        Task<PagedResult<T>> QueryAsync(
            PageQuery query,
            Expression<Func<T, string>>[]? searchableExpressions = null,
            Expression<Func<T, bool>>[]? extraFilters = null);
    }
}
