using Business.Interfaces.Implements.Business;
using Entity.DTOs.Implements.Business.ObligationMonth;
using Hangfire;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using WebGESCOMPH.Controllers.Module.Business;

namespace Test.Modulo.Web;

public class ObligationMonthsControllerTests
{
    private readonly Mock<IObligationMonthService> _svc = new();
    private readonly Mock<IBackgroundJobClient> _jobs = new();
    private readonly Mock<IConfiguration> _cfg = new();
    private readonly Mock<ILogger<ObligationMonthsController>> _logger = new();

    private ObligationMonthsController Create() => new(_svc.Object, _jobs.Object, _cfg.Object, _logger.Object);

    [Fact]
    public async Task Generate_BadRequest_WhenInvalidMonth()
    {
        var res = await Create().Generate(2024, 13, CancellationToken.None);
        Assert.IsType<BadRequestObjectResult>(res);
    }

    [Fact]
    public void Enqueue_BadRequest_WhenInvalidMonth()
    {
        var res = Create().Enqueue(2024, 0);
        Assert.IsType<BadRequestObjectResult>(res);
    }

    [Fact(Skip="Requiere Hangfire JobStorage, se prueba en integración")]
    public void TriggerRecurring_NoContent()
    {
        var res = Create().TriggerRecurring();
        Assert.IsType<NoContentResult>(res);
    }

    // Generic BaseController endpoints
    [Fact]
    public async Task Get_ReturnsOk()
    {
        _svc.Setup(s => s.GetAllAsync())
            .ReturnsAsync(new List<ObligationMonthSelectDto> { new() });
        var res = await Create().Get();
        Assert.IsType<OkObjectResult>(res.Result);
    }

    [Fact]
    public async Task GetById_NotFound()
    {
        _svc.Setup(s => s.GetByIdAsync(9)).ReturnsAsync((ObligationMonthSelectDto?)null);
        var res = await Create().GetById(9);
        Assert.IsType<NotFoundResult>(res.Result);
    }

    [Fact]
    public async Task Post_CreatedAt()
    {
        var created = new ObligationMonthSelectDto { Id = 2 };
        _svc.Setup(s => s.CreateAsync(It.IsAny<ObligationMonthDto>())).ReturnsAsync(created);
        var res = await Create().Post(new ObligationMonthDto { ContractId = 1, Year = 2024, Month = 1 });
        var obj = Assert.IsType<ObjectResult>(res.Result);
        Assert.Equal(201, obj.StatusCode);
    }

    [Fact]
    public async Task Put_Ok()
    {
        var updated = new ObligationMonthSelectDto { Id = 3 };
        _svc.Setup(s => s.UpdateAsync(It.IsAny<ObligationMonthUpdateDto>())).ReturnsAsync(updated);
        var res = await Create().Put(3, new ObligationMonthUpdateDto { Id = 3 });
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
        var res = await Create().ChangeActiveStatus(6, new  WebGESCOMPH.Contracts.Requests.ChangeActiveStatusRequest { Active = true });
        Assert.IsType<NoContentResult>(res);
    }
}
