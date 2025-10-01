using Business.Interfaces.Implements.Location;
using Entity.DTOs.Implements.Location.Department;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using WebGESCOMPAH.Controllers.Module.Locations;

namespace Tests.Web.Locations;

public class DepartmentControllerTests
{
    private readonly Mock<IDepartmentService> _service = new();
    private readonly Mock<ILogger<DepartmentController>> _logger = new();

    private DepartmentController CreateController() => new(_service.Object, _logger.Object);

    [Fact]
    public async Task Get_ReturnsOk()
    {
        _service.Setup(s => s.GetAllAsync()).ReturnsAsync(new List<DepartmentSelectDto> { new() { Id = 1, Name = "Huila" } });
        var res = await CreateController().Get();
        Assert.IsType<OkObjectResult>(res.Result);
    }

    [Fact]
    public async Task GetById_NotFound()
    {
        _service.Setup(s => s.GetByIdAsync(7)).ReturnsAsync((DepartmentSelectDto?)null);
        var controller = CreateController();

        var result = await controller.GetById(7);
        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task Post_CreatedAt()
    {
        var input = new DepartmentCreateDto { Name = "Huila" };
        var created = new DepartmentSelectDto { Id = 1, Name = "Huila", Active = true };
        _service.Setup(s => s.CreateAsync(input)).ReturnsAsync(created);
        var controller = CreateController();

        var result = await controller.Post(input);
        var createdAt = Assert.IsType<CreatedAtActionResult>(result.Result);
        Assert.Equal(nameof(DepartmentController.GetById), createdAt.ActionName);
        Assert.Same(created, createdAt.Value);
    }

    [Fact]
    public async Task Put_NotFound_WhenServiceReturnsNull()
    {
        _service.Setup(s => s.UpdateAsync(It.IsAny<DepartmentUpdateDto>()))
                .ReturnsAsync((DepartmentSelectDto?)null);
        var res = await CreateController().Put(9, new DepartmentUpdateDto { Id = 9, Name = "X" });
        Assert.IsType<NotFoundResult>(res.Result);
    }

    [Fact]
    public async Task Delete_NoContent_WhenDeleted()
    {
        _service.Setup(s => s.DeleteAsync(3)).ReturnsAsync(true);
        var res = await CreateController().Delete(3);
        Assert.IsType<NoContentResult>(res);
    }

    [Fact]
    public async Task DeleteLogic_NoContent_WhenDeleted()
    {
        _service.Setup(s => s.DeleteLogicAsync(4)).ReturnsAsync(true);
        var res = await CreateController().DeleteLogic(4);
        Assert.IsType<NoContentResult>(res);
    }

    [Fact]
    public async Task ChangeActiveStatus_NoContent()
    {
        var res = await CreateController().ChangeActiveStatus(5, new WebGESCOMPAH.Contracts.Requests.ChangeActiveStatusRequest { Active = true });
        Assert.IsType<NoContentResult>(res);
    }
}
