using Entity.Domain.Models.Implements.Location;

using Entity.DTOs.Implements.Location.City;
using Entity.DTOs.Implements.Location.Department;

using Mapster;

namespace Business.Mapping.Registers
{
    public class LocationMapping : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<City, CitySelectDto>();
            config.NewConfig<Department, DepartmentSelectDto>();
        }
    }
}

