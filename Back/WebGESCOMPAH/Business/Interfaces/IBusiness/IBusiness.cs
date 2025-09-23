using Entity.DTOs.Base;

namespace Business.Interfaces.IBusiness
{
    public interface IBusiness<TDtoGet, TDtoCreate, TDtoUpdate>
    {
        Task<IEnumerable<TDtoGet>> GetAllAsync();
        Task<TDtoGet?> GetByIdAsync(int id);
        Task<TDtoGet> CreateAsync(TDtoCreate dto);
        Task<TDtoGet> UpdateAsync(TDtoUpdate dto);
        Task<bool> DeleteAsync(int id);
        Task<bool> DeleteLogicAsync(int id);
        //Task<TDtoGet> UpdateActiveStatusAsync(int id, bool active);
        Task UpdateActiveStatusAsync(int id, bool active);


        Task<PagedResult<TDtoGet>> QueryAsync(PageQuery query);
    }
}
