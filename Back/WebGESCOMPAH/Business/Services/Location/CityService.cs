using Business.Interfaces.Implements.Location;
using Business.Repository;
using Data.Interfaz.DataBasic;
using Data.Interfaz.IDataImplement.Location;
using Entity.Domain.Models.Implements.Location;
using Entity.DTOs.Implements.Location.City;
using MapsterMapper;
using System.Linq.Expressions;

namespace Business.Services.Location
{
    public class CityService : BusinessGeneric<CitySelectDto, CityCreateDto, CityUpdateDto, City>, ICityService
    {
        private readonly ICityRepository _cityRepository;
        public CityService(IDataGeneric<City> data, IMapper mapper, ICityRepository cityRepository) : base(data, mapper)
        {
            _cityRepository = cityRepository;
        }

        protected override IQueryable<City>? ApplyUniquenessFilter(IQueryable<City> query, City candidate)
            => query.Where(c => c.Name == candidate.Name && c.DepartmentId == candidate.DepartmentId);

        public async Task<IEnumerable<CitySelectDto>> GetCityByDepartment(int idDepartment)
        {
            try
            {
                var entities = await _cityRepository.GetCityByDepartmentAsync(idDepartment);
                return _mapper.Map<IEnumerable<CitySelectDto>>(entities);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener las ciudades por departamento", ex);
            }

        }

        protected override Expression<Func<City, string>>[] SearchableFields() => [
            c => c.Name,
            c => c.Department.Name
        ];

        protected override string[] SortableFields() => new[]
        {
            nameof(City.Name),
            nameof(City.DepartmentId),
            nameof(City.CreatedAt),
            nameof(City.Id),
            nameof(City.Active)
        };

        protected override IDictionary<string, LambdaExpression> SortMap()
            => new Dictionary<string, LambdaExpression>(StringComparer.OrdinalIgnoreCase)
            {
                [nameof(City.Name)]          = (Expression<Func<City, string>>)(c => c.Name),
                [nameof(City.DepartmentId)]  = (Expression<Func<City, int>>)(c => c.DepartmentId),
                ["Department.Name"]         = (Expression<Func<City, string>>)(c => c.Department.Name),
                [nameof(City.Active)]        = (Expression<Func<City, bool>>)(c => c.Active),
                [nameof(City.CreatedAt)]     = (Expression<Func<City, DateTime>>)(c => c.CreatedAt),
                [nameof(City.Id)]            = (Expression<Func<City, int>>)(c => c.Id),
            };

        protected override IDictionary<string, Func<string, Expression<Func<City, bool>>>> AllowedFilters() =>
            new Dictionary<string, Func<string, Expression<Func<City, bool>>>>(StringComparer.OrdinalIgnoreCase)
            {
                [nameof(City.Name)] = value => c => c.Name == value,
                [nameof(City.DepartmentId)] = value => c => c.DepartmentId == int.Parse(value),
                [nameof(City.Active)] = value => c => c.Active == bool.Parse(value)
            };

    }
}
