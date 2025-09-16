using Business.Interfaces.Implements.SecurityAuthentication;
using Entity.DTOs.Implements.SecurityAuthentication.User;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Web.Api.Controllers.SecurityAuthentication;

namespace Tests.Web.SecurityAuthentication;

public class UserControllerTests
{
    private readonly Mock<IUserService> _svc = new();
    private readonly Mock<ILogger<UserController>> _logger = new();
    private UserController Create() => new(_svc.Object, _logger.Object);

    [Fact]
    public async Task GetById_NotFound()
    {
        _svc.Setup(s => s.GetByIdAsync(7)).ReturnsAsync((UserSelectDto?)null);
        var res = await Create().GetById(7);
        Assert.IsType<NotFoundResult>(res.Result);
    }

    [Fact]
    public async Task Post_CreatedAt()
    {
        var created = new UserSelectDto { Id = 5, Email = "e@mail" };
        _svc.Setup(s => s.CreateWithPersonAndRolesAsync(It.IsAny<UserCreateDto>())).ReturnsAsync(created);
        var res = await Create().Post(new UserCreateDto { Email = "e@mail", FirstName = "A", LastName = "B", Document = "1", CityId = 1, Phone = "P", Address = "X" });
        Assert.IsType<CreatedAtActionResult>(res.Result);
    }

    [Fact]
    public async Task Put_Ok()
    {
        var updated = new UserSelectDto { Id = 6, Email = "e@mail" };
        _svc.Setup(s => s.UpdateWithPersonAndRolesAsync(It.IsAny<UserUpdateDto>())).ReturnsAsync(updated);
        var res = await Create().Put(6, new UserUpdateDto { Id = 6, Email = "e@mail", FirstName = "A", LastName = "B", Phone = "P", Address = "X", CityId = 1 });
        Assert.IsType<OkObjectResult>(res.Result);
    }

    [Fact]
    public async Task Get_ReturnsOk()
    {
        _svc.Setup(s => s.GetAllAsync()).ReturnsAsync(new List<UserSelectDto> { new() { Id = 1, Email = "a@mail" } });
        var res = await Create().Get();
        Assert.IsType<OkObjectResult>(res.Result);
    }

    [Fact]
    public async Task Delete_NoContent_WhenDeleted()
    {
        _svc.Setup(s => s.DeleteAsync(3)).ReturnsAsync(true);
        var res = await Create().Delete(3);
        Assert.IsType<NoContentResult>(res);
    }

    [Fact]
    public async Task DeleteLogic_NoContent_WhenDeleted()
    {
        _svc.Setup(s => s.DeleteLogicAsync(4)).ReturnsAsync(true);
        var res = await Create().DeleteLogic(4);
        Assert.IsType<NoContentResult>(res);
    }

    [Fact]
    public async Task ChangeActiveStatus_NoContent()
    {
        var res = await Create().ChangeActiveStatus(5, new WebGESCOMPAH.Contracts.Requests.ChangeActiveStatusRequest { Active = true });
        Assert.IsType<NoContentResult>(res);
    }
}
