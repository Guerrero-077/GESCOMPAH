using Business.Services.Business;
using Data.Interfaz.DataBasic;
using Entity.Domain.Models.Implements.Business;
using Entity.DTOs.Implements.Business.Clause;
using MapsterMapper;
using Moq;
using Utilities.Exceptions;

namespace Test.Modulo.Business;

public class ClauseServiceMoreTests
{
    private readonly Mock<IDataGeneric<Clause>> _repo = new();
    private readonly Mock<IMapper> _mapper = new();
    private readonly ClauseService _service;

    public ClauseServiceMoreTests()
    {
        _service = new ClauseService(_repo.Object, _mapper.Object);
    }

    [Fact]
    public async Task Delete_Throws_WhenIdZero()
    {
        var ex = await Assert.ThrowsAsync<BusinessException>(() => _service.DeleteAsync(0));
        Assert.Contains("mayor que cero", ex.InnerException!.Message);
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
        var entity = new Clause { Id = 4, Name = "C", Description = "D", Active = false };
        _repo.Setup(r => r.GetByIdAsync(4)).ReturnsAsync(entity);
        await _service.UpdateActiveStatusAsync(4, true);
        _repo.Verify(r => r.UpdateAsync(It.Is<Clause>(m => m.Id == 4 && m.Active == true)), Times.Once);
    }
}

