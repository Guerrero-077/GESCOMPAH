using Business.Interfaces.Implements.SecurityAuthentication;
using Entity.DTOs.Implements.SecurityAuthentication.RolUser;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using WebGESCOMPAH.Controllers.Module.SecurityAuthentication;

namespace Tests.Web.SecurityAuthentication;

public class RolUserControllerTests
{
    private readonly Mock<IRolUserService> _service = new();
    private readonly Mock<ILogger<RolUserController>> _logger = new();
    private RolUserController Create() => new(_service.Object, _logger.Object);

    [Fact]
    public async Task GetById_Ok_WhenExists()
    {
        _service.Setup(s => s.GetByIdAsync(2)).ReturnsAsync(new RolUserSelectDto { Id = 2 });
        var res = await Create().GetById(2);
        Assert.IsType<OkObjectResult>(res.Result);
    }

    [Fact]
    public async Task Get_ReturnsOk()
    {
        _service.Setup(s => s.GetAllAsync()).ReturnsAsync(new List<RolUserSelectDto> { new() { Id = 1 } });
        var res = await Create().Get();
        Assert.IsType<OkObjectResult>(res.Result);
    }

    [Fact]
    public async Task Post_CreatedAt()
    {
        var created = new RolUserSelectDto { Id = 3 };
        _service.Setup(s => s.CreateAsync(It.IsAny<RolUserCreateDto>())).ReturnsAsync(created);
        var res = await Create().Post(new RolUserCreateDto { RolId = 1, UserId = 1 });
        Assert.IsType<CreatedAtActionResult>(res.Result);
    }

    [Fact]
    public async Task Put_Ok()
    {
        var updated = new RolUserSelectDto { Id = 4 };
        _service.Setup(s => s.UpdateAsync(It.IsAny<RolUserUpdateDto>())).ReturnsAsync(updated);
        var res = await Create().Put(4, new RolUserUpdateDto { Id = 4 });
        Assert.IsType<OkObjectResult>(res.Result);
    }

    [Fact]
    public async Task Delete_NoContent_WhenDeleted()
    {
        _service.Setup(s => s.DeleteAsync(5)).ReturnsAsync(true);
        var res = await Create().Delete(5);
        Assert.IsType<NoContentResult>(res);
    }

    [Fact]
    public async Task DeleteLogic_NoContent_WhenDeleted()
    {
        _service.Setup(s => s.DeleteLogicAsync(6)).ReturnsAsync(true);
        var res = await Create().DeleteLogic(6);
        Assert.IsType<NoContentResult>(res);
    }

    [Fact]
    public async Task ChangeActiveStatus_NoContent()
    {
        var res = await Create().ChangeActiveStatus(7, new WebGESCOMPAH.Contracts.Requests.ChangeActiveStatusRequest { Active = true });
        Assert.IsType<NoContentResult>(res);
    }
}
