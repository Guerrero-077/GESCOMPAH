using Business.Services.AdministrationSystem;
using Data.Interfaz.DataBasic;
using Entity.Domain.Models.Implements.AdministrationSystem;
using Entity.DTOs.Implements.AdministrationSystem.Module;
using MapsterMapper;
using Moq;
using Utilities.Exceptions;

namespace Tests.Business.AdministrationSystem;

public class ModuleServiceMoreTests
{
    private readonly Mock<IDataGeneric<Module>> _repo = new();
    private readonly Mock<IMapper> _mapper = new();
    private readonly ModuleService _service;

    public ModuleServiceMoreTests()
    {
        _service = new ModuleService(_repo.Object, _mapper.Object);
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

    [Fact]
    public async Task UpdateActiveStatus_Updates_WhenFound()
    {
        var entity = new Module { Id = 3, Name = "M", Description = "D", Active = false, Icon = "i" };
        _repo.Setup(r => r.GetByIdAsync(3)).ReturnsAsync(entity);
        await _service.UpdateActiveStatusAsync(3, true);
        _repo.Verify(r => r.UpdateAsync(It.Is<Module>(m => m.Id == 3 && m.Active == true)), Times.Once);
    }
}

