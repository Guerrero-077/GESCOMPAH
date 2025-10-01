using Business.Services.AdministrationSystem;
using Data.Interfaz.DataBasic;
using Entity.Domain.Models.Implements.AdministrationSystem;
using Entity.DTOs.Implements.AdministrationSystem.Module;
using MapsterMapper;
using Moq;

namespace Test.Modulo.Business;

public class ModuleServiceTests
{
    private readonly Mock<IDataGeneric<Module>> _repo = new();
    private readonly Mock<IMapper> _mapper = new();
    private readonly ModuleService _service;

    public ModuleServiceTests()
    {
        _service = new ModuleService(_repo.Object, _mapper.Object);
    }

    [Fact]
    public async Task GetAll_MapsDtos()
    {
        var entities = new List<Module> { new() { Id = 1, Name = "M" } };
        _repo.Setup(r => r.GetAllAsync()).ReturnsAsync(entities);
        _mapper.Setup(m => m.Map<IEnumerable<ModuleSelectDto>>(entities)).Returns(new List<ModuleSelectDto> { new() { Id = 1, Name = "M" } });
        var res = await _service.GetAllAsync();
        Assert.Single(res);
    }
}

