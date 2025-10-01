using Business.Interfaces.Implements.SecurityAuthentication;
using Entity.DTOs.Implements.SecurityAuthentication.Permission;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using WebGESCOMPH.Controllers.Module.SecurityAuthentication;

namespace Test.Modulo.Web;

public class PermissionControllerTests
{
    private readonly Mock<IPermissionService> _service = new();
    private readonly Mock<ILogger<PermissionController>> _logger = new();

    private PermissionController CreateController() => new(_service.Object, _logger.Object);

    [Fact]
    public async Task DeleteLogic_ReturnsNoContent_WhenDeleted()
    {
        _service.Setup(s => s.DeleteLogicAsync(5)).ReturnsAsync(true);
        var result = await CreateController().DeleteLogic(5);
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task Get_ReturnsOk()
    {
        _service.Setup(s => s.GetAllAsync()).ReturnsAsync(new List<PermissionSelectDto> { new() { Id = 1, Name = "P" } });
        var res = await CreateController().Get();
        Assert.IsType<OkObjectResult>(res.Result);
    }

    [Fact]
    public async Task GetById_NotFound()
    {
        _service.Setup(s => s.GetByIdAsync(10)).ReturnsAsync((PermissionSelectDto?)null);
        var res = await CreateController().GetById(10);
        Assert.IsType<NotFoundResult>(res.Result);
    }

    [Fact]
    public async Task Post_CreatedAt()
    {
        var created = new PermissionSelectDto { Id = 3, Name = "P" };
        _service.Setup(s => s.CreateAsync(It.IsAny<PermissionCreateDto>())).ReturnsAsync(created);
        var res = await CreateController().Post(new PermissionCreateDto { Name = "P" });
        Assert.IsType<CreatedAtActionResult>(res.Result);
    }

    [Fact]
    public async Task Put_Ok()
    {
        var updated = new PermissionSelectDto { Id = 4, Name = "UP" };
        _service.Setup(s => s.UpdateAsync(It.IsAny<PermissionUpdateDto>())).ReturnsAsync(updated);
        var res = await CreateController().Put(4, new PermissionUpdateDto { Id = 4, Name = "UP" });
        Assert.IsType<OkObjectResult>(res.Result);
    }

    [Fact]
    public async Task Delete_NoContent_WhenDeleted()
    {
        _service.Setup(s => s.DeleteAsync(6)).ReturnsAsync(true);
        var res = await CreateController().Delete(6);
        Assert.IsType<NoContentResult>(res);
    }

    [Fact]
    public async Task ChangeActiveStatus_NoContent()
    {
        var res = await CreateController().ChangeActiveStatus(7, new WebGESCOMPH.Contracts.Requests.ChangeActiveStatusRequest { Active = true });
        Assert.IsType<NoContentResult>(res);
    }
}
