using Business.Interfaces.Implements.Business;
using Entity.DTOs.Implements.Business.EstablishmentDto;
using Microsoft.AspNetCore.Mvc;
using Moq;
using WebGESCOMPH.Controllers.Module.Business;

namespace Test.Modulo.Web;

public class EstablishmentsControllerTests
{
    private readonly Mock<IEstablishmentService> _svc = new();
    private EstablishmentsController Create() => new(_svc.Object);

    [Fact]
    public async Task GetAll_ActiveOnly_UsesActiveService()
    {
        _svc.Setup(s => s.GetAllActiveAsync()).ReturnsAsync(new List<EstablishmentSelectDto> { new() { Id = 1 } });
        var res = await Create().GetAll(activeOnly: true);
        var ok = Assert.IsType<OkObjectResult>(res.Result);
        var list = Assert.IsAssignableFrom<IEnumerable<EstablishmentSelectDto>>(ok.Value);
        Assert.Single(list);
    }

    [Fact]
    public async Task GetById_NotFound_WhenMissing()
    {
        _svc.Setup(s => s.GetByIdAnyAsync(5)).ReturnsAsync((EstablishmentSelectDto?)null);
        var res = await Create().GetById(5, activeOnly: false);
        Assert.IsType<NotFoundResult>(res);
    }

    [Fact]
    public async Task Update_BadRequest_WhenIdInvalid()
    {
        var res = await Create().Update(0, new EstablishmentUpdateDto());
        Assert.IsType<BadRequestObjectResult>(res.Result);
    }
}

