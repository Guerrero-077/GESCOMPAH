using Business.Interfaces.Implements.SecurityAuthentication;
using Entity.DTOs.Implements.SecurityAuthentication.Rol;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using WebGESCOMPAH.Contracts.Requests;

public class RolControllerTests
{
    private readonly RolController _controller;
    private readonly Mock<IRolService> _serviceMock;

    public RolControllerTests()
    {
        _serviceMock = new Mock<IRolService>();
        var loggerMock = new Mock<ILogger<RolController>>();
        _controller = new RolController(_serviceMock.Object, loggerMock.Object);
    }

    [Fact]
    public async Task Get_All()
    {
        // Arrange
        var fakeData = new List<RolSelectDto>
        {
            new RolSelectDto { Id = 1, Name = "Admin", Active = true },
            new RolSelectDto { Id = 2, Name = "Producer", Active = true },
        };

        _serviceMock.Setup(s => s.GetAllAsync()).ReturnsAsync(fakeData);

        // Act
        var result = await _controller.Get();

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result.Result); 
        var payload = Assert.IsAssignableFrom<IEnumerable<RolSelectDto>>(ok.Value);
        Assert.Equal(2, payload.Count());
        Assert.Contains(payload, r => r.Name == "Admin");
    }

    [Fact]
    public async Task GetById_WhenFound()
    {
        // Arrange
        var rol = new RolSelectDto { Id = 10, Name = "QA", Active = true };
        _serviceMock.Setup(s => s.GetByIdAsync(10)).ReturnsAsync(rol);

        // Act
        var result = await _controller.GetById(10);

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var payload = Assert.IsType<RolSelectDto>(ok.Value);
        Assert.Equal(10, payload.Id);
    }

    [Fact]
    public async Task GetById_NotFound()
    {
        // Arrange
        _serviceMock.Setup(s => s.GetByIdAsync(999)).ReturnsAsync((RolSelectDto?)null);

        // Act
        var result = await _controller.GetById(999);

        // Assert
        Assert.IsType<NotFoundResult>(result.Result); 
    }

    [Fact]
    public async Task Delete_WhenServiceReturnsTrue()
    {
        // Arrange
        _serviceMock.Setup(s => s.DeleteAsync(7)).ReturnsAsync(true);

        // Act
        var result = await _controller.Delete(7);

        // Assert
        Assert.IsType<NoContentResult>(result); 
    }

    [Fact]
    public async Task Delete_WhenServiceReturnsFalse()
    {
        // Arrange
        _serviceMock.Setup(s => s.DeleteAsync(7)).ReturnsAsync(false);

        // Act
        var result = await _controller.Delete(7);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Post_ReturnCreated()
    {
        // Arrange
        var dto = new RolCreateDto
        {
            Name = "NewRole",
            Description = "Test role"
        };

        var created = new RolSelectDto
        {
            Id = 99,
            Name = dto.Name,
            Description = dto.Description,
            Active = true
        };

        _serviceMock.Setup(s => s.CreateAsync(dto)).ReturnsAsync(created);

        // Act
        var result = await _controller.Post(dto);

        // Assert
        var createdAt = Assert.IsType<CreatedAtActionResult>(result.Result);
        var payload = Assert.IsType<RolSelectDto>(createdAt.Value);
        Assert.Equal(created.Id, payload.Id);
        Assert.Equal("NewRole", payload.Name);
    }

    [Fact]
    public async Task Put_WhenUpdatedSuccessfully()
    {
        // Arrange
        var dto = new RolUpdateDto { Id = 5, Name = "Admin", Description = "Administrador" };

        var updated = new RolSelectDto { Id = 5, Name = "Admin", Active = true };

        _serviceMock.Setup(s => s.UpdateAsync(It.IsAny<RolUpdateDto>()))
                    .ReturnsAsync(updated);

        // Act
        var result = await _controller.Put(5, dto);

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result.Result); 
        var payload = Assert.IsType<RolSelectDto>(ok.Value);
        Assert.Equal(5, payload.Id);
    }

    [Fact]
    public async Task Put_WhenServiceReturnsNull()
    {
        // Arrange
        var dto = new RolUpdateDto { Id = 8, Name = "Admin", Description = "Administrador" };

        _serviceMock.Setup(s => s.UpdateAsync(It.IsAny<RolUpdateDto>()))
                    .ReturnsAsync((RolSelectDto?)null);

        // Act
        var result = await _controller.Put(8, dto);

        // Assert
        Assert.IsType<NotFoundResult>(result.Result); 
    }

    [Fact]
    public async Task ChangeActiveStatus_WhenSuccess()
    {
        var id = 3;
        var request = new ChangeActiveStatusRequest { Active = false };

        _serviceMock
            .Setup(s => s.UpdateActiveStatusAsync(id, false))
            .Returns(Task.CompletedTask);

        var result = await _controller.ChangeActiveStatus(id, request);

        Assert.IsType<NoContentResult>(result);
    }
}
