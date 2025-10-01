using Business.Interfaces.Implements.Location;
using Entity.DTOs.Implements.Location.City;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using WebGESCOMPH.Controllers.Module.Locations;

namespace Test.Modulo.Web;

public class CityControllerTests
{
    private readonly Mock<ICityService> _service = new();
    private readonly Mock<ILogger<CityController>> _logger = new();

    private CityController CreateController() => new(_service.Object, _logger.Object);

    [Fact]
    public async Task Get_ReturnsOk()
    {
        _service.Setup(s => s.GetAllAsync()).ReturnsAsync(new List<CitySelectDto> { new() { Id = 1, Name = "Neiva" } });
        var result = await CreateController().Get();
        Assert.IsType<OkObjectResult>(result.Result);
    }

    [Fact]
    public async Task GetById_NotFound()
    {
        _service.Setup(s => s.GetByIdAsync(9)).ReturnsAsync((CitySelectDto?)null);
        var res = await CreateController().GetById(9);
        Assert.IsType<NotFoundResult>(res.Result);
    }

    [Fact]
    public async Task Post_CreatedAt()
    {
        var created = new CitySelectDto { Id = 2, Name = "Campoalegre" };
        _service.Setup(s => s.CreateAsync(It.IsAny<CityCreateDto>())).ReturnsAsync(created);
        var res = await CreateController().Post(new CityCreateDto { Name = "Campoalegre" });
        Assert.IsType<CreatedAtActionResult>(res.Result);
    }

    [Fact]
    public async Task Put_Ok()
    {
        var updated = new CitySelectDto { Id = 3, Name = "Rivera" };
        _service.Setup(s => s.UpdateAsync(It.IsAny<CityUpdateDto>())).ReturnsAsync(updated);
        var res = await CreateController().Put(3, new CityUpdateDto { Id = 3, Name = "Rivera" });
        Assert.IsType<OkObjectResult>(res.Result);
    }

    [Fact]
    public async Task Delete_NoContent_WhenDeleted()
    {
        _service.Setup(s => s.DeleteAsync(4)).ReturnsAsync(true);
        var res = await CreateController().Delete(4);
        Assert.IsType<NoContentResult>(res);
    }

    [Fact]
    public async Task DeleteLogic_NoContent_WhenDeleted()
    {
        _service.Setup(s => s.DeleteLogicAsync(5)).ReturnsAsync(true);
        var res = await CreateController().DeleteLogic(5);
        Assert.IsType<NoContentResult>(res);
    }

    [Fact]
    public async Task ChangeActiveStatus_NoContent()
    {
        var res = await CreateController().ChangeActiveStatus(6, new WebGESCOMPH.Contracts.Requests.ChangeActiveStatusRequest { Active = true });
        Assert.IsType<NoContentResult>(res);
    }
}
