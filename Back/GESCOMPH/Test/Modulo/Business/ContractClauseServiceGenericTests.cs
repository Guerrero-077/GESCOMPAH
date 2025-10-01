using Business.Services.Business;
using Data.Interfaz.DataBasic;
using Entity.Domain.Models.Implements.Business;
using Entity.DTOs.Implements.Business.ContractClause;
using MapsterMapper;
using Moq;
using Utilities.Exceptions;

namespace Test.Modulo.Business;

public class ContractClauseServiceGenericTests
{
    private readonly Mock<IDataGeneric<ContractClause>> _repo = new();
    private readonly Mock<IMapper> _mapper = new();
    private readonly ContractClauseService _service;

    public ContractClauseServiceGenericTests()
    {
        _service = new ContractClauseService(_repo.Object, _mapper.Object);
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
        var entity = new ContractClause { Id = 4, ContractId = 1, ClauseId = 1, Active = false };
        _repo.Setup(r => r.GetByIdAsync(4)).ReturnsAsync(entity);
        await _service.UpdateActiveStatusAsync(4, true);
        _repo.Verify(r => r.UpdateAsync(It.Is<ContractClause>(m => m.Id == 4 && m.Active == true)), Times.Once);
    }
}

