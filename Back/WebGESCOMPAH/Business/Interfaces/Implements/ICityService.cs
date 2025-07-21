using Business.Interfaces.IBusiness;
using Entity.DTOs.Implements.Location.Select;

namespace Business.Interfaces.Implements
{
    public interface ICityService : IBusiness<CitySelectDto, CitySelectDto>
    {
        Task<IEnumerable<CitySelectDto>> GetCityByDepartment(int idDepartment);
    }
}
