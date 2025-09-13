using Business.Services.Location;
using Data.Interfaz.DataBasic;
using Data.Interfaz.IDataImplement.Location;
using Entity.Domain.Models.Implements.Location;
using Entity.DTOs.Implements.Location.City;
using MapsterMapper;
using Moq;

namespace Tests.Business.Locations;

public class CityServiceTests
{
    private readonly Mock<IDataGeneric<City>> _repo = new();
    private readonly Mock<IMapper> _mapper = new();
    private readonly Mock<ICityRepository> _cityRepo = new();
    private readonly CityService _service;

    public CityServiceTests()
    {
        _service = new CityService(_repo.Object, _mapper.Object, _cityRepo.Object);
    }

    [Fact]
    public async Task GetCityByDepartment_MapsEntities()
    {
        var entities = new List<City> { new() { Id = 1, Name = "Neiva" } };
        _cityRepo.Setup(r => r.GetCityByDepartmentAsync(41)).ReturnsAsync(entities);
        _mapper.Setup(m => m.Map<IEnumerable<CitySelectDto>>(entities))
               .Returns(new List<CitySelectDto> { new() { Id = 1, Name = "Neiva" } });

        var result = await _service.GetCityByDepartment(41);
        Assert.Single(result);
    }
}

