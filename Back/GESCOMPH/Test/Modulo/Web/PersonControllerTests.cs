using Business.Interfaces.Implements.Persons;
using Entity.DTOs.Implements.Persons.Person;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using WebGESCOMPH.Controllers.Module.Persons;

namespace Test.Modulo.Web;

public class PersonControllerTests
{
    private readonly Mock<IPersonService> _service = new();
    private readonly Mock<ILogger<PersonController>> _logger = new();
    private PersonController Create() => new(_service.Object, _logger.Object);

    [Fact]
    public async Task Put_Ok_CallsService()
    {
        var dto = new PersonUpdateDto { Id = 0, FirstName = "X", LastName = "Y", Address = "A", Phone = "P", CityId = 1 };
        var updated = new PersonSelectDto { Id = 7, FirstName = "X", LastName = "Y", CityId = 1, CityName = "C" };
        _service.Setup(s => s.UpdateAsync(It.IsAny<PersonUpdateDto>())).ReturnsAsync(updated);
        var res = await Create().Put(7, dto);
        var ok = Assert.IsType<OkObjectResult>(res.Result);
        Assert.Same(updated, ok.Value);
        _service.Verify(s => s.UpdateAsync(It.IsAny<PersonUpdateDto>()), Times.Once);
    }

    [Fact]
    public async Task Get_ReturnsOk()
    {
        _service.Setup(s => s.GetAllAsync()).ReturnsAsync(new List<PersonSelectDto> { new() { Id = 1, FirstName = "A", LastName = "B", CityId = 1, CityName = "C" } });
        var res = await Create().Get();
        Assert.IsType<OkObjectResult>(res.Result);
    }

    [Fact]
    public async Task GetById_NotFound()
    {
        _service.Setup(s => s.GetByIdAsync(10)).ReturnsAsync((PersonSelectDto?)null);
        var res = await Create().GetById(10);
        Assert.IsType<NotFoundResult>(res.Result);
    }

    [Fact]
    public async Task Post_CreatedAt()
    {
        var created = new PersonSelectDto { Id = 2, FirstName = "A", LastName = "B", CityId = 1, CityName = "C" };
        _service.Setup(s => s.CreateAsync(It.IsAny<PersonDto>())).ReturnsAsync(created);
        var res = await Create().Post(new PersonDto { FirstName = "A", LastName = "B", Document = "1", CityId = 1, Address = "X", Phone = "Y" });
        Assert.IsType<CreatedAtActionResult>(res.Result);
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
        var res = await Create().ChangeActiveStatus(7, new WebGESCOMPH.Contracts.Requests.ChangeActiveStatusRequest { Active = true });
        Assert.IsType<NoContentResult>(res);
    }
}
