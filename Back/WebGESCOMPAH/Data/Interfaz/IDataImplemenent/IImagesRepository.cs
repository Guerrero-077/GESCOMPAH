using Data.Interfaz.DataBasic;
using Entity.Domain.Models.Implements.Utilities;

namespace Data.Interfaz.IDataImplemenent
{
    public interface IImagesRepository : IDataGeneric<Images>
    {
        Task AddAsync(List<Images> entity);
        Task<List<Images>> GetByEstablishmentIdAsync(int establishmentId);
        Task DeleteRangeAsync(List<Images> images);
    }
}
