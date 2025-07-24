using Business.Interfaces.IBusiness;
using Entity.DTOs.Implements.Business.Plaza;

namespace Business.Interfaces.Implements.Business
{
    public interface IPlazaService : IBusiness<PlazaSelectDto, PlazaCreateDto, PlazaUpdateDto>
    {
        //Task<IEnumerable<PlazaSelectDto>> GetAllPlazas();
        //Task<PlazaSelectDto> CreatePlazaAsync(PlazaCreateDto plazaDto);
        //Task<PlazaSelectDto> UpdatePlazaAsync(PlazaUpdateDto dto);
        //Task<PlazaSelectDto?> GetPlazaByIdAsync(int id);
        Task<PlazaSelectDto> UpdateActiveStatusAsync(int id, bool active);



    }
}
