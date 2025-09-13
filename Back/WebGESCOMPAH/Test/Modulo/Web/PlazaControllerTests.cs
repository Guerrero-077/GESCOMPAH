using Business.Interfaces.Implements.Business;
using Entity.DTOs.Implements.Business.Plaza;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using WebGESCOMPAH.Controllers.Module.Business;

namespace Tests.Web.Business;

public class PlazaControllerTests
{
    private readonly Mock<IPlazaService> _service = new();
    private readonly Mock<ILogger<PlazaController>> _logger = new();
    private PlazaController Create() => new(_service.Object, _logger.Object);

    [Fact]
    public async Task DeleteLogic_NotFound_WhenServiceReturnsFalse()
    {
        _service.Setup(s => s.DeleteLogicAsync(123)).ReturnsAsync(false);
        var res = await Create().DeleteLogic(123);
        Assert.IsType<NotFoundResult>(res);
    }

    [Fact]
    public async Task Get_ReturnsOk()
    {
        _service.Setup(s => s.GetAllAsync()).ReturnsAsync(new List<PlazaSelectDto> { new() { Id = 1, Name = "P" } });
        var res = await Create().Get();
        Assert.IsType<OkObjectResult>(res.Result);
    }

    [Fact]
    public async Task GetById_NotFound_WhenMissing()
    {
        _service.Setup(s => s.GetByIdAsync(9)).ReturnsAsync((PlazaSelectDto?)null);
        var res = await Create().GetById(9);
        Assert.IsType<NotFoundResult>(res.Result);
    }

    [Fact]
    public async Task Post_CreatedAt()
    {
        var created = new PlazaSelectDto { Id = 2, Name = "NP" };
        _service.Setup(s => s.CreateAsync(It.IsAny<PlazaCreateDto>())).ReturnsAsync(created);
        var res = await Create().Post(new PlazaCreateDto { Name = "NP" });
        Assert.IsType<CreatedAtActionResult>(res.Result);
    }

    [Fact]
    public async Task Put_Ok()
    {
        var updated = new PlazaSelectDto { Id = 3, Name = "UP" };
        _service.Setup(s => s.UpdateAsync(It.IsAny<PlazaUpdateDto>())).ReturnsAsync(updated);
        var res = await Create().Put(3, new PlazaUpdateDto { Id = 3, Name = "UP" });
        Assert.IsType<OkObjectResult>(res.Result);
    }

    [Fact]
    public async Task Delete_NoContent_WhenDeleted()
    {
        _service.Setup(s => s.DeleteAsync(4)).ReturnsAsync(true);
        var res = await Create().Delete(4);
        Assert.IsType<NoContentResult>(res);
    }

    [Fact]
    public async Task ChangeActiveStatus_NoContent()
    {
        var res = await Create().ChangeActiveStatus(7, new WebGESCOMPAH.Contracts.Requests.ChangeActiveStatusRequest { Active = true });
        Assert.IsType<NoContentResult>(res);
    }
}
