using Business.Services.Business;
using Data.Interfaz.DataBasic;
using Entity.Domain.Models.Implements.Business;
using Entity.DTOs.Implements.Business.Plaza;
using MapsterMapper;
using Moq;

namespace Test.Modulo.Business;

public class PlazasServiceTests
{
    private readonly Mock<IDataGeneric<Plaza>> _repo = new();
    private readonly Mock<IMapper> _mapper = new();
    private readonly PlazasService _service;

    public PlazasServiceTests()
    {
        _service = new PlazasService(_repo.Object, _mapper.Object);
    }

    [Fact]
    public async Task UpdateActiveStatus_UpdatesWhenDifferent()
    {
        var entity = new Plaza { Id = 3, Active = false };
        _repo.Setup(r => r.GetByIdAsync(3)).ReturnsAsync(entity);

        await _service.UpdateActiveStatusAsync(3, true);

        _repo.Verify(r => r.UpdateAsync(It.Is<Plaza>(p => p.Id == 3 && p.Active == true)), Times.Once);
    }
}

