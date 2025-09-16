using Business.Interfaces.Implements.AdministrationSystem;
using Entity.DTOs.Implements.AdministrationSystem.Module;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using WebGESCOMPAH.Controllers.Module.AdministrationSystem;

namespace Tests.Web.AdministrationSystem;

public class ModuleControllerTests
{
    private readonly Mock<IModuleService> _service = new();
    private readonly Mock<ILogger<ModuleController>> _logger = new();
    private ModuleController Create() => new(_service.Object, _logger.Object);

    [Fact]
    public async Task Post_ReturnsCreatedAt()
    {
        var input = new ModuleCreateDto { Name = "M" };
        var created = new ModuleSelectDto { Id = 2, Name = "M" };
        _service.Setup(s => s.CreateAsync(input)).ReturnsAsync(created);
        var res = await Create().Post(input);
        Assert.IsType<CreatedAtActionResult>(res.Result);
    }

    [Fact]
    public async Task Get_ReturnsOk()
    {
        _service.Setup(s => s.GetAllAsync()).ReturnsAsync(new List<ModuleSelectDto> { new() { Id = 1 } });
        var res = await Create().Get();
        Assert.IsType<OkObjectResult>(res.Result);
    }

    [Fact]
    public async Task Delete_NoContent_WhenDeleted()
    {
        _service.Setup(s => s.DeleteAsync(1)).ReturnsAsync(true);
        var res = await Create().Delete(1);
        Assert.IsType<NoContentResult>(res);
    }

    [Fact]
    public async Task GetById_NotFound()
    {
        _service.Setup(s => s.GetByIdAsync(9)).ReturnsAsync((ModuleSelectDto?)null);
        var res = await Create().GetById(9);
        Assert.IsType<NotFoundResult>(res.Result);
    }

    [Fact]
    public async Task Put_Ok()
    {
        var updated = new ModuleSelectDto { Id = 3, Name = "M" };
        _service.Setup(s => s.UpdateAsync(It.IsAny<ModuleUpdateDto>())).ReturnsAsync(updated);
        var res = await Create().Put(3, new ModuleUpdateDto { Id = 3, Name = "M" });
        Assert.IsType<OkObjectResult>(res.Result);
    }

    [Fact]
    public async Task DeleteLogic_NoContent()
    {
        _service.Setup(s => s.DeleteLogicAsync(4)).ReturnsAsync(true);
        var res = await Create().DeleteLogic(4);
        Assert.IsType<NoContentResult>(res);
    }

    [Fact]
    public async Task ChangeActiveStatus_NoContent()
    {
        var res = await Create().ChangeActiveStatus(6, new WebGESCOMPAH.Contracts.Requests.ChangeActiveStatusRequest { Active = true });
        Assert.IsType<NoContentResult>(res);
    }
}
