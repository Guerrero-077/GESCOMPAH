using Business.Interfaces.Implements.SecurityAuthentication;
using Entity.DTOs.Implements.SecurityAuthentication.Rol;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using WebGESCOMPH.Controllers.Module.SecurityAuthentication;

namespace Test.Modulo.Web;

public class RolControllerTests
{
    private readonly Mock<IRolService> _service = new();
    private readonly Mock<ILogger<RolController>> _logger = new();

    private RolController CreateController() => new(_service.Object, _logger.Object);

    [Fact]
    public async Task Get_ReturnsOk_WithList()
    {
        // Arrange
        var items = new List<RolSelectDto> { new() { Id = 1, Name = "Admin" } };
        _service.Setup(s => s.GetAllAsync()).ReturnsAsync(items);
        var controller = CreateController();

        // Act
        var result = await controller.Get();

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var model = Assert.IsAssignableFrom<IEnumerable<RolSelectDto>>(ok.Value);
        Assert.Single(model);
    }

    [Fact]
    public async Task GetById_ReturnsOk_WhenFound()
    {
        _service.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(new RolSelectDto { Id = 1, Name = "Admin" });
        var controller = CreateController();

        var result = await controller.GetById(1);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var model = Assert.IsType<RolSelectDto>(ok.Value);
        Assert.Equal(1, model.Id);
    }

    [Fact]
    public async Task GetById_ReturnsNotFound_WhenMissing()
    {
        _service.Setup(s => s.GetByIdAsync(99)).ReturnsAsync((RolSelectDto?)null);
        var controller = CreateController();

        var result = await controller.GetById(99);

        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task Post_ReturnsCreatedAt_WithLocationHeader()
    {
        var input = new RolCreateDto { Name = "New" };
        var created = new RolSelectDto { Id = 10, Name = "New" };
        _service.Setup(s => s.CreateAsync(input)).ReturnsAsync(created);
        var controller = CreateController();

        var result = await controller.Post(input);

        var createdAt = Assert.IsType<CreatedAtActionResult>(result.Result);
        Assert.Equal(nameof(RolController.GetById), createdAt.ActionName);
        Assert.Equal(10, ((dynamic)createdAt.RouteValues!["id"])!);
        Assert.Same(created, createdAt.Value);
    }

    [Fact]
    public async Task Put_ReturnsOk_WhenUpdated()
    {
        var update = new RolUpdateDto { Id = 5, Name = "Upd" };
        var updated = new RolSelectDto { Id = 5, Name = "Upd" };
        _service.Setup(s => s.UpdateAsync(It.IsAny<RolUpdateDto>())).ReturnsAsync(updated);
        var controller = CreateController();

        var result = await controller.Put(5, update);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Same(updated, ok.Value);
        _service.Verify(s => s.UpdateAsync(It.Is<RolUpdateDto>(d => d.Id == 5)), Times.Once);
    }

    [Fact]
    public async Task Delete_ReturnsNoContent_WhenDeleted()
    {
        _service.Setup(s => s.DeleteAsync(3)).ReturnsAsync(true);
        var controller = CreateController();

        var result = await controller.Delete(3);
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task Delete_ReturnsNotFound_WhenMissing()
    {
        _service.Setup(s => s.DeleteAsync(33)).ReturnsAsync(false);
        var controller = CreateController();

        var result = await controller.Delete(33);
        Assert.IsType<NotFoundResult>(result);
    }
}

