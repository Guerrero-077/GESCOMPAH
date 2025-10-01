using Business.Interfaces.IBusiness;
using Entity.DTOs.Implements.Location.City;

namespace Business.Interfaces.Implements.Location
{
    public interface ICityService : IBusiness<CitySelectDto, CityCreateDto, CityUpdateDto>
    {
        Task<IEnumerable<CitySelectDto>> GetCityByDepartment(int idDepartment);
    }
}
