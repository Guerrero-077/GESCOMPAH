using Business.Services.Location;
using Data.Interfaz.DataBasic;
using Entity.Domain.Models.Implements.Location;
using Entity.DTOs.Implements.Location.Department;
using MapsterMapper;
using Moq;
using Utilities.Exceptions;

namespace Tests.Business.Locations;

public class DepartmentServiceTests
{
    private readonly Mock<IDataGeneric<Department>> _repo = new();
    private readonly Mock<IMapper> _mapper = new();
    private readonly DepartmentService _service;

    public DepartmentServiceTests()
    {
        _service = new DepartmentService(_repo.Object, _mapper.Object);
    }

    [Fact]
    public async Task GetAll_ReturnsDtos()
    {
        var entities = new List<Department> { new() { Id = 1, Name = "Huila" } };
        _repo.Setup(r => r.GetAllAsync()).ReturnsAsync(entities);
        _mapper.Setup(m => m.Map<IEnumerable<DepartmentSelectDto>>(entities))
               .Returns(new List<DepartmentSelectDto> { new() { Id = 1, Name = "Huila", Active = true } });

        var result = await _service.GetAllAsync();
        Assert.Single(result);
    }

    [Fact]
    public async Task GetById_Throws_WhenIdZero()
    {
        var ex = await Assert.ThrowsAsync<BusinessException>(() => _service.GetByIdAsync(0));
        Assert.Contains("ID 0", ex.Message);
    }

    [Fact]
    public async Task UpdateActiveStatus_Throws_WhenNotFound()
    {
        _repo.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Department?)null);
        var ex = await Assert.ThrowsAsync<BusinessException>(() => _service.UpdateActiveStatusAsync(99, true));
        Assert.IsType<KeyNotFoundException>(ex.InnerException);
    }

    [Fact]
    public async Task UpdateActiveStatus_Updates_WhenFound()
    {
        var dep = new Department { Id = 5, Name = "D", Active = false };
        _repo.Setup(r => r.GetByIdAsync(5)).ReturnsAsync(dep);
        await _service.UpdateActiveStatusAsync(5, true);
        _repo.Verify(r => r.UpdateAsync(It.Is<Department>(d => d.Id == 5 && d.Active == true)), Times.Once);
    }

    [Fact]
    public async Task Delete_Throws_WhenIdZero()
    {
        var ex = await Assert.ThrowsAsync<BusinessException>(() => _service.DeleteAsync(0));
        Assert.Contains("mayor que cero", ex.InnerException!.Message);
    }

    [Fact]
    public async Task Delete_Wraps_DbUpdateException()
    {
        _repo.Setup(r => r.DeleteAsync(1)).ThrowsAsync(new Microsoft.EntityFrameworkCore.DbUpdateException("FK"));
        var ex = await Assert.ThrowsAsync<BusinessException>(() => _service.DeleteAsync(1));
        Assert.Contains("restricciones de datos", ex.Message);
    }

    [Fact]
    public async Task DeleteLogic_Throws_WhenIdZero()
    {
        var ex = await Assert.ThrowsAsync<BusinessException>(() => _service.DeleteLogicAsync(0));
        Assert.Contains("mayor que cero", ex.InnerException!.Message);
    }
}
