using Business.Interfaces.Implements.Business;
using Entity.DTOs.Implements.Business.ContractClause;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using WebGESCOMPAH.Controllers.Module.Business;

namespace Tests.Web.Business;

public class ContractClauseControllerTests
{
    private readonly Mock<IContractClauseService> _service = new();
    private readonly Mock<ILogger<ContractClauseController>> _logger = new();
    private ContractClauseController Create() => new(_service.Object, _logger.Object);

    [Fact]
    public async Task Get_ReturnsOk()
    {
        _service.Setup(s => s.GetAllAsync()).ReturnsAsync(new List<ContractClauseSelectDto> { new() { Id = 1 } });
        var res = await Create().Get();
        Assert.IsType<OkObjectResult>(res.Result);
    }

    [Fact]
    public async Task GetById_Ok_WhenFound()
    {
        _service.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(new ContractClauseSelectDto { Id = 1 });
        var res = await Create().GetById(1);
        Assert.IsType<OkObjectResult>(res.Result);
    }

    [Fact]
    public async Task GetById_NotFound_WhenMissing()
    {
        _service.Setup(s => s.GetByIdAsync(9)).ReturnsAsync((ContractClauseSelectDto?)null);
        var res = await Create().GetById(9);
        Assert.IsType<NotFoundResult>(res.Result);
    }

    [Fact]
    public async Task Post_CreatedAt()
    {
        var created = new ContractClauseSelectDto { Id = 3 };
        _service.Setup(s => s.CreateAsync(It.IsAny<ContractClauseDto>())).ReturnsAsync(created);
        var res = await Create().Post(new ContractClauseDto { ContractId = 1, ClauseId = 1 });
        Assert.IsType<CreatedAtActionResult>(res.Result);
    }

    [Fact]
    public async Task Put_ReturnsOk()
    {
        var updated = new ContractClauseSelectDto { Id = 2 };
        _service.Setup(s => s.UpdateAsync(It.IsAny<ContractClauseUpdateDto>())).ReturnsAsync(updated);
        var res = await Create().Put(2, new ContractClauseUpdateDto { Id = 2, ClauseId = 1, ContractId = 1 });
        Assert.IsType<OkObjectResult>(res.Result);
    }

    [Fact]
    public async Task Put_NotFound_WhenNull()
    {
        _service.Setup(s => s.UpdateAsync(It.IsAny<ContractClauseUpdateDto>())).ReturnsAsync((ContractClauseSelectDto?)null);
        var res = await Create().Put(2, new ContractClauseUpdateDto { Id = 2 });
        Assert.IsType<NotFoundResult>(res.Result);
    }

    [Fact]
    public async Task Delete_NoContent_WhenDeleted()
    {
        _service.Setup(s => s.DeleteAsync(2)).ReturnsAsync(true);
        var res = await Create().Delete(2);
        Assert.IsType<NoContentResult>(res);
    }

    [Fact]
    public async Task Delete_NotFound_WhenMissing()
    {
        _service.Setup(s => s.DeleteAsync(22)).ReturnsAsync(false);
        var res = await Create().Delete(22);
        Assert.IsType<NotFoundResult>(res);
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
