using Business.Interfaces.Implements.AdministrationSystem;
using Entity.DTOs.Implements.AdministrationSystem.Form;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using WebGESCOMPAH.Controllers.Module.AdministrationSystem;

namespace Tests.Web.AdministrationSystem;

public class FormControllerTests
{
    private readonly Mock<IFormService> _service = new();
    private readonly Mock<ILogger<FormController>> _logger = new();
    private FormController Create() => new(_service.Object, _logger.Object);

    [Fact]
    public async Task Get_ReturnsOk()
    {
        _service.Setup(s => s.GetAllAsync()).ReturnsAsync(new List<FormSelectDto> { new() { Id = 1 } });
        var res = await Create().Get();
        Assert.IsType<OkObjectResult>(res.Result);
    }

    [Fact]
    public async Task DeleteLogic_NoContent()
    {
        _service.Setup(s => s.DeleteLogicAsync(8)).ReturnsAsync(true);
        var res = await Create().DeleteLogic(8);
        Assert.IsType<NoContentResult>(res);
    }
}
