using Business.Interfaces.Implements.SecurityAuthentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using WebGESCOMPAH.Controllers.Module.SecurityAuthentication;

namespace Tests.Web.SecurityAuthentication;

public class RolFormPermissionControllerTests
{
    private readonly Mock<IRolFormPermissionService> _svc = new();
    private readonly Mock<ILogger<RolFormPermissionController>> _logger = new();
    private RolFormPermissionController Create() => new(_svc.Object, _logger.Object);

    [Fact]
    public async Task GetAllGrouped_Ok()
    {
        _svc.Setup(s => s.GetAllGroupedAsync()).ReturnsAsync(new List<Entity.DTOs.Implements.SecurityAuthentication.RolFormPemission.RolFormPermissionSelectDto>());
        var res = await Create().GetAllGrouped();
        Assert.IsType<OkObjectResult>(res);
    }

    [Fact]
    public async Task DeleteByGroup_NoContent_WhenDeleted()
    {
        _svc.Setup(s => s.DeleteByGroupAsync(1, 2)).ReturnsAsync(true);
        var res = await Create().DeleteByGroup(1, 2);
        Assert.IsType<NoContentResult>(res);
    }

    [Fact]
    public async Task DeleteByGroup_NotFound_WhenMissing()
    {
        _svc.Setup(s => s.DeleteByGroupAsync(1, 999)).ReturnsAsync(false);
        var res = await Create().DeleteByGroup(1, 999);
        Assert.IsType<NotFoundObjectResult>(res);
    }

    // Generic BaseController endpoints
    [Fact]
    public async Task Get_ReturnsOk()
    {
        _svc.Setup(s => s.GetAllAsync())
            .ReturnsAsync(new List<Entity.DTOs.Implements.SecurityAuthentication.RolFormPemission.RolFormPermissionSelectDto> { new() { Id = 1 } });
        var res = await Create().Get();
        Assert.IsType<OkObjectResult>(res.Result);
    }

    [Fact]
    public async Task GetById_NotFound()
    {
        _svc.Setup(s => s.GetByIdAsync(10)).ReturnsAsync((Entity.DTOs.Implements.SecurityAuthentication.RolFormPemission.RolFormPermissionSelectDto?)null);
        var res = await Create().GetById(10);
        Assert.IsType<NotFoundResult>(res.Result);
    }

    [Fact]
    public async Task Post_CreatedAt()
    {
        var created = new Entity.DTOs.Implements.SecurityAuthentication.RolFormPemission.RolFormPermissionSelectDto { Id = 2 };
        _svc.Setup(s => s.CreateAsync(It.IsAny<Entity.DTOs.Implements.SecurityAuthentication.RolFormPemission.RolFormPermissionCreateDto>()))
            .ReturnsAsync(created);
        var res = await Create().Post(new Entity.DTOs.Implements.SecurityAuthentication.RolFormPemission.RolFormPermissionCreateDto { RolId = 1, FormId = 1 });
        Assert.IsType<CreatedAtActionResult>(res.Result);
    }

    [Fact]
    public async Task Put_Ok()
    {
        var updated = new Entity.DTOs.Implements.SecurityAuthentication.RolFormPemission.RolFormPermissionSelectDto { Id = 3 };
        _svc.Setup(s => s.UpdateAsync(It.IsAny<Entity.DTOs.Implements.SecurityAuthentication.RolFormPemission.RolFormPermissionUpdateDto>()))
            .ReturnsAsync(updated);
        var res = await Create().Put(3, new Entity.DTOs.Implements.SecurityAuthentication.RolFormPemission.RolFormPermissionUpdateDto { Id = 3 });
        Assert.IsType<OkObjectResult>(res.Result);
    }

    [Fact]
    public async Task Delete_NoContent_WhenDeleted()
    {
        _svc.Setup(s => s.DeleteAsync(4)).ReturnsAsync(true);
        var res = await Create().Delete(4);
        Assert.IsType<NoContentResult>(res);
    }

    [Fact]
    public async Task DeleteLogic_NoContent_WhenDeleted()
    {
        _svc.Setup(s => s.DeleteLogicAsync(5)).ReturnsAsync(true);
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
