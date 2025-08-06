using Data.Interfaz.DataBasic;
using Entity.Domain.Models.Implements.Utilities;

namespace Data.Interfaz.IDataImplemenent.Utilities
{
    public interface IImagesRepository : IDataGeneric<Images>
    {
        Task AddAsync(List<Images> entity);
        Task<List<Images>> GetByEstablishmentIdAsync(int establishmentId);
        Task<bool> DeleteByPublicIdAsync(string publicId);
        //Task DeleteRangeAsync(List<Images> images);
    }
}
