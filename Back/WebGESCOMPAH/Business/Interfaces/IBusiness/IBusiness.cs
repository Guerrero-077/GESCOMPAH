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

    }
}
