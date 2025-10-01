using Business.Interfaces.Implements.Business;
using Entity.DTOs.Implements.Business.Contract;
using Business.Services.Business;
using Data.Interfaz.IDataImplement.Business;
using Business.Interfaces.Implements.Persons;
using Business.CustomJWT;
using Business.Interfaces.Implements.SecurityAuthentication;
using Business.Interfaces;
using Business.Interfaces.PDF;
using Data.Interfaz.IDataImplement.Persons;
using Data.Interfaz.IDataImplement.SecurityAuthentication;
using Entity.Domain.Models.Implements.Business;
using Entity.Domain.Models.Implements.Persons;
using Entity.Domain.Models.Implements.SecurityAuthentication;
using Entity.DTOs.Implements.Business.ObligationMonth;
using Entity.Infrastructure.Context;
using MapsterMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Utilities.Exceptions;
using Utilities.Messaging.Interfaces;

namespace Test.Modulo.Business;

public class ContractServiceTests
{
    private readonly Mock<IContractRepository> _contracts = new();
    private readonly Mock<IObligationMonthRepository> _obligationRepo = new();
    private readonly Mock<IObligationMonthService> _obligationSvc = new();
    private readonly Mock<IPersonService> _personSvc = new();
    private readonly Mock<IPersonRepository> _personRepo = new();
    private readonly Mock<IUserRepository> _userRepo = new();
    private readonly Mock<IRolUserRepository> _rolUserRepo = new();
    private readonly Mock<IPremisesLeasedRepository> _premRepo = new();
    private readonly Mock<IEstablishmentsRepository> _estRepo = new();
    private readonly Mock<IEstablishmentService> _estSvc = new();
    private readonly Mock<IPasswordHasher<User>> _hasher = new();
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
        var opt = new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        _ctx = new ApplicationDbContext(opt);

        _service = new ContractService(
            _contracts.Object,
            _personSvc.Object,
            _personRepo.Object,
            _userRepo.Object,
            _rolUserRepo.Object,
            _premRepo.Object,
            _estRepo.Object,
            _estSvc.Object,
            _mapper.Object,
            _hasher.Object,
            _email.Object,
            _ctx,
            _currentUser.Object,
            _obligationRepo.Object,
            _obligationSvc.Object,
            _userCtx.Object,
            _contractPdfService.Object,
            _uow.Object,
            _logger.Object
        );
    }

    [Fact]
    public async Task GetMine_Admin_ReturnsMappedCards()
    {
        _currentUser.SetupGet(u => u.EsAdministrador).Returns(true);
        var cards = new List<ContractCardDto>
        {
            new(1, 10, "P", "D", "PH", null, DateTime.Today, DateTime.Today, 1, 1, true)
        };
        _contracts.Setup(r => r.GetCardsAllAsync()).ReturnsAsync(cards);

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
        _contracts.Setup(r => r.GetCardsByPersonAsync(99)).ReturnsAsync(cards);

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
    public async Task GetObligations_MapsList()
    {
        var optLocal = new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        await using var ctxLocal = new ApplicationDbContext(optLocal);
        ctxLocal.obligationMonths.Add(new ObligationMonth { Id = 1, ContractId = 5, Year = 2024, Month = 1, DueDate = DateTime.Today, UvtQtyApplied = 0, UvtValueApplied = 0, VatRateApplied = 0, BaseAmount = 0, VatAmount = 0, TotalAmount = 0, Status = "PENDING" });
        ctxLocal.obligationMonths.Add(new ObligationMonth { Id = 2, ContractId = 6, Year = 2024, Month = 1, DueDate = DateTime.Today, UvtQtyApplied = 0, UvtValueApplied = 0, VatRateApplied = 0, BaseAmount = 0, VatAmount = 0, TotalAmount = 0, Status = "PENDING" });
        await ctxLocal.SaveChangesAsync();

        _obligationRepo.Setup(r => r.GetByContractQueryable(5))
                       .Returns(ctxLocal.obligationMonths.Where(o => o.ContractId == 5));
        _mapper.Setup(m => m.Map<List<ObligationMonthSelectDto>>(It.IsAny<List<ObligationMonth>>()))
               .Returns(new List<ObligationMonthSelectDto> { new() });

        var result = await _service.GetObligationsAsync(5);
        Assert.Single(result);
    }

    [Fact(Skip="EF InMemory no soporta transacciones; cubrir en integración relacional")]
    public async Task RunExpirationSweep_ReturnsRepositoryResults()
    {
        _contracts.Setup(r => r.DeactivateExpiredAsync(It.IsAny<DateTime>())).ReturnsAsync(new List<int> { 1, 2 });
        _contracts.Setup(r => r.ReleaseEstablishmentsForExpiredAsync(It.IsAny<DateTime>())).ReturnsAsync(3);

        var result = await _service.RunExpirationSweepAsync(CancellationToken.None);
        Assert.Equal(2, result.DeactivatedContractIds.Count);
        Assert.Equal(3, result.ReactivatedEstablishments);
    }
}
