namespace Business.Interfaces.IBusiness
{
    public interface IBusiness<TDto, TDtoGet>
    {
        Task<IEnumerable<TDtoGet>> GetAllAsync();
        Task<TDtoGet?> GetByIdAsync(int id);
        Task<TDto> CreateAsync(TDto dto);
        Task<bool> UpdateAsync(TDto dto);
        Task<bool> DeleteAsync(int id);
        Task<bool> DeleteLogicAsync(int id);

    }
}
