using Business.Services.Business;
using Data.Interfaz.DataBasic;
using Entity.Domain.Models.Implements.Business;
using Entity.DTOs.Implements.Business.Clause;
using MapsterMapper;
using Moq;
using Utilities.Exceptions;

namespace Test.Modulo.Business;

public class ClauseServiceTests
{
    private readonly Mock<IDataGeneric<Clause>> _repo = new();
    private readonly Mock<IMapper> _mapper = new();
    private readonly ClauseService _service;

    public ClauseServiceTests()
    {
        _service = new ClauseService(_repo.Object, _mapper.Object);
    }

    [Fact]
    public async Task Delete_ThrowsBusiness_OnDbUpdate()
    {
        _repo.Setup(r => r.DeleteAsync(1)).ThrowsAsync(new Microsoft.EntityFrameworkCore.DbUpdateException("fk"));
        var ex = await Assert.ThrowsAsync<BusinessException>(() => _service.DeleteAsync(1));
        Assert.Contains("restricciones de datos", ex.Message);
    }
}

