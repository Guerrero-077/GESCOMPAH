using Business.Interfaces.Implements.Business;
using Business.Interfaces.PDF;
using Entity.DTOs.Implements.Business.Contract;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Moq;
using WebGESCOMPAH.Controllers.Module.Business;
using WebGESCOMPAH.RealTime;

namespace Tests.Web.Business;

public class ContractControllerTests
{
    private readonly Mock<IContractService> _service = new();
    private readonly Mock<IContractPdfGeneratorService> _pdf = new();
    private readonly Mock<ILogger<ContractController>> _logger = new();
    private readonly Mock<IHubContext<ContractsHub>> _hub = new();

    private ContractController Create() => new(_service.Object, _pdf.Object, _logger.Object, _hub.Object);

    [Fact]
    public async Task GetMine_ReturnsOk()
    {
        _service.Setup(s => s.GetMineAsync()).ReturnsAsync(new List<ContractCardDto>());
        var res = await Create().GetMine();
        Assert.IsType<OkObjectResult>(res);
    }

    [Fact]
    public async Task DownloadContractPdf_NotFound_WhenMissing()
    {
        _service.Setup(s => s.GetByIdAsync(77)).ReturnsAsync((ContractSelectDto?)null);
        var res = await Create().DownloadContractPdf(77);
        Assert.IsType<NotFoundObjectResult>(res);
    }

    private void SetupHub()
    {
        var clients = new Mock<IHubClients>();
        var all = new Mock<IClientProxy>();
        clients.Setup(c => c.All).Returns(all.Object);
        all.Setup(p => p.SendCoreAsync(It.IsAny<string>(), It.IsAny<object?[]>(), default)).Returns(Task.CompletedTask);
        _hub.SetupGet(h => h.Clients).Returns(clients.Object);
    }

    [Fact]
    public async Task Post_ReturnsCreatedAt_WithRouteId()
    {
        SetupHub();
        _service.Setup(s => s.CreateContractWithPersonHandlingAsync(It.IsAny<ContractCreateDto>())).ReturnsAsync(42);
        var res = await Create().Post(new ContractCreateDto {
            FirstName = "A",
            LastName = "B",
            Document = "1",
            Phone = "P",
            Address = "Addr",
            CityId = 1,
            EstablishmentIds = new List<int> { 1 }
        });
        var created = Assert.IsType<CreatedAtActionResult>(res.Result);
        Assert.Equal(nameof(ContractController.GetById), created.ActionName);
        Assert.Equal(42, (int)created.RouteValues!["id"]!);
    }

    [Fact]
    public async Task ChangeActiveStatus_NoContent()
    {
        SetupHub();
        var res = await Create().ChangeActiveStatus(5, new WebGESCOMPAH.Contracts.Requests.ChangeActiveStatusRequest { Active = true });
        Assert.IsType<NoContentResult>(res);
    }

    [Fact]
    public async Task Delete_NoContent()
    {
        SetupHub();
        _service.Setup(s => s.DeleteAsync(3)).ReturnsAsync(true);
        var res = await Create().Delete(3);
        Assert.IsType<NoContentResult>(res);
    }

    [Fact]
    public async Task GetObligations_NotFound_WhenContractMissing()
    {
        _service.Setup(s => s.GetByIdAsync(9)).ReturnsAsync((ContractSelectDto?)null);
        var res = await Create().GetObligations(9);
        Assert.IsType<NotFoundResult>(res);
    }

    [Fact]
    public async Task GetObligations_Ok_WhenExists()
    {
        _service.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(new ContractSelectDto());
        _service.Setup(s => s.GetObligationsAsync(1)).ReturnsAsync(new List<Entity.DTOs.Implements.Business.ObligationMonth.ObligationMonthSelectDto> { new() });
        var res = await Create().GetObligations(1);
        Assert.IsType<OkObjectResult>(res);
    }

    [Fact]
    public async Task DownloadContractPdf_Ok_ReturnsPdf()
    {
        _service.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(new ContractSelectDto { FullName = "X" });
        _pdf.Setup(p => p.GeneratePdfAsync(It.IsAny<ContractSelectDto>())).ReturnsAsync(new byte[] { 1, 2, 3 });
        var res = await Create().DownloadContractPdf(1);
        var file = Assert.IsType<FileContentResult>(res);
        Assert.Equal("application/pdf", file.ContentType);
        Assert.NotEmpty(file.FileContents);
    }
}
