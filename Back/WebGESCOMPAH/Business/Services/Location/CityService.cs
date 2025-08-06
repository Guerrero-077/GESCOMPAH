using Business.Interfaces.Implements.Location;
using Business.Repository;
using Data.Interfaz.DataBasic;
using Data.Interfaz.IDataImplemenent.Location;
using Entity.Domain.Models.Implements.Location;
using Entity.DTOs.Implements.Location.City;
using MapsterMapper;

namespace Business.Services.Location
{
    public class CityService : BusinessGeneric<CitySelectDto, CityCreateDto, CityUpdateDto, City>, ICityService
    {
        private readonly ICityRepository _cityRepository;
        public CityService(IDataGeneric<City> data, IMapper mapper, ICityRepository cityRepository) : base(data, mapper)
        {
            _cityRepository = cityRepository;
        }

        public async Task<IEnumerable<CitySelectDto>> GetCityByDepartment(int idDepartment)
        {
            try
            {
                var entities = await _cityRepository.GetCityByDepartment(idDepartment);
                return _mapper.Map<IEnumerable<CitySelectDto>>(entities);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener las ciudades por departamento", ex);
            }

        }
    }
}