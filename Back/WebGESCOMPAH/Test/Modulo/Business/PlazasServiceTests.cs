using Business.Services.Business;
using Data.Interfaz.DataBasic;
using Data.Interfaz.IDataImplement.Business;
using Entity.Domain.Models.Implements.Business;
using Entity.DTOs.Implements.Business.Plaza;
using MapsterMapper;
using Moq;

namespace Tests.Business.Business;

public class PlazasServiceTests
{
    private readonly Mock<IDataGeneric<Plaza>> _repo = new();
    private readonly Mock<IMapper> _mapper = new();
    private readonly Mock<IEstablishmentsRepository> _estRepo = new();
    private readonly Mock<IContractRepository> _contractRepo = new();
    private readonly PlazasService _service;

    public PlazasServiceTests()
    {
        _service = new PlazasService(_repo.Object, _mapper.Object, _estRepo.Object, _contractRepo.Object);
    }

    [Fact]
    public async Task UpdateActiveStatus_UpdatesWhenDifferent()
    {
        var entity = new Plaza { Id = 3, Active = false };
        _repo.Setup(r => r.GetByIdAsync(3)).ReturnsAsync(entity);

        await _service.UpdateActiveStatusAsync(3, true);

        _repo.Verify(r => r.UpdateAsync(It.Is<Plaza>(p => p.Id == 3 && p.Active == true)), Times.Once);
    }
    
    [Fact]
    public async Task UpdateActiveStatus_Disable_CascadesToEstablishments()
    {
        var entity = new Plaza { Id = 5, Active = true };
        _repo.Setup(r => r.GetByIdAsync(5)).ReturnsAsync(entity);
        _contractRepo.Setup(r => r.AnyActiveByPlazaAsync(5)).ReturnsAsync(false);

        await _service.UpdateActiveStatusAsync(5, false);

        _repo.Verify(r => r.UpdateAsync(It.Is<Plaza>(p => p.Id == 5 && p.Active == false)), Times.Once);
        _estRepo.Verify(r => r.SetActiveByPlazaIdAsync(5, false), Times.Once);
    }

    [Fact]
    public async Task UpdateActiveStatus_Disable_Blocked_WhenActiveContracts()
    {
        var entity = new Plaza { Id = 7, Active = true };
        _repo.Setup(r => r.GetByIdAsync(7)).ReturnsAsync(entity);
        _contractRepo.Setup(r => r.AnyActiveByPlazaAsync(7)).ReturnsAsync(true);

        await Assert.ThrowsAsync<Utilities.Exceptions.BusinessException>(() => _service.UpdateActiveStatusAsync(7, false));

        _repo.Verify(r => r.UpdateAsync(It.IsAny<Plaza>()), Times.Never);
        _estRepo.Verify(r => r.SetActiveByPlazaIdAsync(It.IsAny<int>(), It.IsAny<bool>()), Times.Never);
    }
}

