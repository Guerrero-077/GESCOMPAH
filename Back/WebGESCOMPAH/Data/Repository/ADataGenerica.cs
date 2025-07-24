using Data.Interfaz.DataBasic;
using Entity.Domain.Models.ModelBase;

namespace Data.Repository
{
    public abstract class ADataGenerica<T> : IDataGeneric<T> where T : BaseModel
    {
        public abstract Task<IEnumerable<T>> GetAllAsync();
        public abstract Task<T?> GetByIdAsync(int id);
        public abstract Task<T> AddAsync(T entity);
        public abstract Task<T> UpdateAsync(T entity);
        public abstract Task<bool> DeleteAsync(int id);
        public abstract Task<bool> DeleteLogicAsync(int id);
        public abstract IQueryable<T> GetAllQueryable();
    }
}
