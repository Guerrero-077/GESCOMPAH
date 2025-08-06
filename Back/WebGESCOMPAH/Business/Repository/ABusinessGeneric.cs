using Business.Interfaces.IBusiness;
using Entity.Domain.Models.ModelBase;

namespace Business.Repository
{
    public abstract class ABusinessGeneric<TDtoGet, TDtoCreate, TDtoUpdate, TEntity> : IBusiness<TDtoGet, TDtoCreate, TDtoUpdate> where TEntity : BaseModel
    {
        public abstract Task<IEnumerable<TDtoGet>> GetAllAsync();
        public abstract Task<TDtoGet?> GetByIdAsync(int id);
        public abstract Task<TDtoGet> CreateAsync(TDtoCreate dto);
        public abstract Task<TDtoUpdate> UpdateAsync(TDtoUpdate dto);
        public abstract Task<bool> DeleteAsync(int id);
        public abstract Task<bool> DeleteLogicAsync(int id);
    }
}
