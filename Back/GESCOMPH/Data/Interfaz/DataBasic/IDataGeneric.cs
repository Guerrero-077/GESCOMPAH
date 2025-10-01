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
    }
}
