using Business.Interfaces.IBusiness;
using Entity.Domain.Models.ModelBase;

namespace Business.Repository
{
    public abstract class ABusinessGeneric<TDto, TDtoGet, TEntity> : IBusiness<TDto, TDtoGet> where TEntity : BaseModel
    {
        public abstract Task<IEnumerable<TDtoGet>> GetAllAsync();
        public abstract Task<TDtoGet?> GetByIdAsync(int id);
        public abstract Task<TDto> CreateAsync(TDto dto);
        public abstract Task<bool> UpdateAsync(TDto dto);
        public abstract Task<bool> DeleteAsync(int id);
        public abstract Task<bool> DeleteLogicAsync(int id);
    }
}
