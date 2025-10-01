using Business.Interfaces.Implements.AdministrationSystem;
using Entity.DTOs.Implements.AdministrationSystem.FormModule;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using WebGESCOMPAH.Controllers.Module.AdministrationSystem;

namespace Tests.Web.AdministrationSystem;

public class FormModuleControllerTests
{
    private readonly Mock<IFormMouduleService> _service = new();
    private readonly Mock<ILogger<FormModuleController>> _logger = new();
    private FormModuleController Create() => new(_service.Object, _logger.Object);

    [Fact]
    public async Task GetById_NotFound()
    {
        _service.Setup(s => s.GetByIdAsync(88)).ReturnsAsync((FormModuleSelectDto?)null);
        var res = await Create().GetById(88);
        Assert.IsType<NotFoundResult>(res.Result);
    }

    [Fact]
    public async Task Get_ReturnsOk()
    {
        _service.Setup(s => s.GetAllAsync()).ReturnsAsync(new List<FormModuleSelectDto> { new() { Id = 1 } });
        var res = await Create().Get();
        Assert.IsType<OkObjectResult>(res.Result);
    }

    [Fact]
    public async Task Post_CreatedAt()
    {
        var created = new FormModuleSelectDto { Id = 2 };
        _service.Setup(s => s.CreateAsync(It.IsAny<FormModuleCreateDto>())).ReturnsAsync(created);
        var res = await Create().Post(new FormModuleCreateDto { FormId = 1, ModuleId = 1 });
        Assert.IsType<CreatedAtActionResult>(res.Result);
    }

    [Fact]
    public async Task Put_Ok()
    {
        var updated = new FormModuleSelectDto { Id = 3 };
        _service.Setup(s => s.UpdateAsync(It.IsAny<FormModuleUpdateDto>())).ReturnsAsync(updated);
        var res = await Create().Put(3, new FormModuleUpdateDto { Id = 3 });
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
    public async Task DeleteLogic_NoContent_WhenDeleted()
    {
        _service.Setup(s => s.DeleteLogicAsync(5)).ReturnsAsync(true);
        var res = await Create().DeleteLogic(5);
        Assert.IsType<NoContentResult>(res);
    }

    [Fact]
    public async Task ChangeActiveStatus_NoContent()
    {
        var res = await Create().ChangeActiveStatus(6, new WebGESCOMPAH.Contracts.Requests.ChangeActiveStatusRequest { Active = true });
        Assert.IsType<NoContentResult>(res);
    }
}
