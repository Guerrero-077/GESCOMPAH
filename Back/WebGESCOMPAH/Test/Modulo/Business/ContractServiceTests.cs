using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Business.Interfaces;
using Business.Interfaces.Implements.Business;
using Business.Interfaces.Implements.Persons;
using Business.Interfaces.Implements.SecurityAuthentication;
using Business.Interfaces.PDF;
using Business.Services.Business;
using Business.CustomJWT;
using Data.Interfaz.IDataImplement.Business;
using Entity.Domain.Models.Implements.Business;
using Entity.DTOs.Implements.Business.Contract;
using Entity.DTOs.Implements.Business.ObligationMonth;
using Entity.Infrastructure.Context;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Utilities.Exceptions;
using Utilities.Messaging.Interfaces;
using Xunit;

namespace Tests.Business.Business;

public class ContractServiceTests
{
    private readonly Mock<IContractRepository> _contracts = new();
    private readonly Mock<IObligationMonthService> _obligationSvc = new();
    private readonly Mock<IPersonService> _personSvc = new();
    private readonly Mock<IEstablishmentService> _estSvc = new();
    private readonly Mock<IUserService> _userSvc = new();
    private readonly Mock<ISendCode> _email = new();
    private readonly Mock<IMapper> _mapper = new();
    private readonly Mock<ICurrentUser> _currentUser = new();
    private readonly Mock<IUserContextService> _userCtx = new();
    private readonly Mock<IContractPdfGeneratorService> _contractPdfService = new();
    private readonly Mock<IUnitOfWork> _uow = new();
    private readonly Mock<ILogger<ContractService>> _logger = new();
    private readonly ApplicationDbContext _ctx;
    private readonly ContractService _service;

    public ContractServiceTests()
    {
        var opt = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        _ctx = new ApplicationDbContext(opt);

        _service = new ContractService(
            _contracts.Object,
            _personSvc.Object,
            _estSvc.Object,
            _userSvc.Object,
            _mapper.Object,
            _email.Object,
            _ctx,
            _currentUser.Object,
            _obligationSvc.Object,
            _userCtx.Object,
            _contractPdfService.Object,
            _uow.Object,
            _logger.Object);
    }

    [Fact]
    public async Task GetMine_Admin_ReturnsMappedCards()
    {
        _currentUser.SetupGet(u => u.EsAdministrador).Returns(true);
        var cards = new List<ContractCardDto>
        {
            new(1, 10, "P", "D", "PH", null, DateTime.Today, DateTime.Today, 1, 1, true)
        };
        _contracts.Setup(r => r.GetCardsAllAsync()).ReturnsAsync((IReadOnlyList<ContractCardDto>)cards);

        var result = await _service.GetMineAsync();
        Assert.Single(result);
    }

    [Fact]
    public async Task GetMine_NonAdmin_WithoutPerson_Throws()
    {
        _currentUser.SetupGet(u => u.EsAdministrador).Returns(false);
        _currentUser.SetupGet(u => u.PersonId).Returns((int?)null);
        await Assert.ThrowsAsync<BusinessException>(() => _service.GetMineAsync());
    }

    [Fact]
    public async Task GetMine_NonAdmin_WithPerson_ReturnsMappedCards()
    {
        _currentUser.SetupGet(u => u.EsAdministrador).Returns(false);
        _currentUser.SetupGet(u => u.PersonId).Returns(99);
        var cards = new List<ContractCardDto>
        {
            new(2, 99, "Z", "D", "PH", null, DateTime.Today, DateTime.Today, 1, 1, false)
        };
        _contracts.Setup(r => r.GetCardsByPersonAsync(99)).ReturnsAsync((IReadOnlyList<ContractCardDto>)cards);

        var result = await _service.GetMineAsync();
        Assert.Single(result);
        Assert.Equal(99, result[0].PersonId);
    }

    [Fact]
    public async Task GetObligations_InvalidId_Throws()
    {
        await Assert.ThrowsAsync<BusinessException>(() => _service.GetObligationsAsync(0));
    }

    [Fact]
    public async Task GetObligations_DelegatesToService()
    {
        _obligationSvc.Setup(s => s.GetByContractAsync(5))
            .ReturnsAsync(new List<ObligationMonthSelectDto> { new() });

        var result = await _service.GetObligationsAsync(5);

        Assert.Single(result);
        _obligationSvc.Verify(s => s.GetByContractAsync(5), Times.Once);
    }

    [Fact(Skip = "EF InMemory no soporta transacciones; cubrir en integracion relacional")]
    public async Task RunExpirationSweep_ReturnsRepositoryResults()
    {
        _contracts.Setup(r => r.DeactivateExpiredAsync(It.IsAny<DateTime>())).ReturnsAsync(new List<int> { 1, 2 });
        _contracts.Setup(r => r.ReleaseEstablishmentsForExpiredAsync(It.IsAny<DateTime>())).ReturnsAsync(3);

        var result = await _service.RunExpirationSweepAsync(CancellationToken.None);
        Assert.Equal(2, result.DeactivatedContractIds.Count);
        Assert.Equal(3, result.ReactivatedEstablishments);
    }
}



