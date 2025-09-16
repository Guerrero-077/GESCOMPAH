using Business.Interfaces.Implements.AdministrationSystem;
using Entity.DTOs.Implements.AdministrationSystem.SystemParameter;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using WebGESCOMPAH.Controllers.Module.AdministrationSystem;

namespace Tests.Web.AdministrationSystem;

public class SystemParameterControllerTests
{
    private readonly Mock<ISystemParameterService> _service = new();
    private readonly Mock<ILogger<SystemParameterController>> _logger = new();
    private SystemParameterController Create() => new(_service.Object, _logger.Object);

    [Fact]
    public async Task Delete_ReturnsNoContent_WhenDeleted()
    {
        _service.Setup(s => s.DeleteAsync(4)).ReturnsAsync(true);
        var res = await Create().Delete(4);
        Assert.IsType<NoContentResult>(res);
    }

    [Fact]
    public async Task Get_ReturnsOk()
    {
        _service.Setup(s => s.GetAllAsync()).ReturnsAsync(new List<SystemParameterSelectDto> { new() { Id = 1 } });
        var res = await Create().Get();
        Assert.IsType<OkObjectResult>(res.Result);
    }

    [Fact]
    public async Task GetById_NotFound()
    {
        _service.Setup(s => s.GetByIdAsync(9)).ReturnsAsync((SystemParameterSelectDto?)null);
        var res = await Create().GetById(9);
        Assert.IsType<NotFoundResult>(res.Result);
    }

    [Fact]
    public async Task Post_CreatedAt()
    {
        var created = new SystemParameterSelectDto { Id = 2 };
        _service.Setup(s => s.CreateAsync(It.IsAny<SystemParameterDto>())).ReturnsAsync(created);
        var res = await Create().Post(new SystemParameterDto { Key = "K", Value = "V" });
        var obj = Assert.IsType<ObjectResult>(res.Result);
        Assert.Equal(201, obj.StatusCode);
    }

    [Fact]
    public async Task Put_Ok()
    {
        var updated = new SystemParameterSelectDto { Id = 3 };
        _service.Setup(s => s.UpdateAsync(It.IsAny<SystemParameterUpdateDto>())).ReturnsAsync(updated);
        var res = await Create().Put(3, new SystemParameterUpdateDto { Id = 3 });
        Assert.IsType<OkObjectResult>(res.Result);
    }

    [Fact]
    public async Task DeleteLogic_NoContent()
    {
        _service.Setup(s => s.DeleteLogicAsync(5)).ReturnsAsync(true);
        var res = await Create().DeleteLogic(5);
        Assert.IsType<NoContentResult>(res);
    }

    [Fact]
    public async Task ChangeActiveStatus_NoContent()
    {
        var res = await Create().ChangeActiveStatus(7, new WebGESCOMPAH.Contracts.Requests.ChangeActiveStatusRequest { Active = true });
        Assert.IsType<NoContentResult>(res);
    }
}
