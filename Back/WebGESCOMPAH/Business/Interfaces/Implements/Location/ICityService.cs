using Business.Interfaces.IBusiness;
using Entity.DTOs.Implements.Location.City;

namespace Business.Interfaces.Implements.Location
{
    public interface ICityService : IBusiness<CitySelectDto, CitySelectDto, CitySelectDto>
    {
        Task<IEnumerable<CitySelectDto>> GetCityByDepartment(int idDepartment);
    }
}
