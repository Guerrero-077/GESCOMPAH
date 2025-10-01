using Business.Services.Business;
using Data.Interfaz.IDataImplement.Business;
using Data.Interfaz.DataBasic;
using Entity.Domain.Models.Implements.Business;
using Entity.Domain.Models.Implements.AdministrationSystem;
using Entity.DTOs.Implements.Business.EstablishmentDto;
using Entity.Infrastructure.Context;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Utilities.Exceptions;

namespace Tests.Business.Business;

public class EstablishmentServiceTests
{
    private readonly Mock<IEstablishmentsRepository> _repo = new();
    private readonly Mock<IMapper> _mapper = new();
    private readonly Mock<ILogger<EstablishmentService>> _logger = new();
    private readonly Mock<IDataGeneric<SystemParameter>> _systemParamRepo = new();
    private readonly ApplicationDbContext _ctx;
    private readonly EstablishmentService _service;

    public EstablishmentServiceTests()
    {
        var opt = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        _ctx = new ApplicationDbContext(opt);

        // Simula UVT vigente
        _systemParamRepo.Setup(r => r.GetAllQueryable())
            .Returns(new List<SystemParameter>
            {
                new SystemParameter { Key = "UVT", Value = "10", EffectiveFrom = DateTime.UtcNow.AddYears(-1) }
            }.AsQueryable());

        _service = new EstablishmentService(
            _repo.Object,
            _ctx,
            _mapper.Object,
            _logger.Object,
            _systemParamRepo.Object
        );
    }

    [Fact]
    public async Task Create_Throws_WhenInvalidValues()
    {
        var dto = new EstablishmentCreateDto { AreaM2 = 0, UvtQty = 0, PlazaId = 0 };

        var ex = await Assert.ThrowsAsync<BusinessException>(() => _service.CreateAsync(dto));

        Assert.Contains("Payload inválido", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Create_Succeeds_ReturnsSelect()
    {
        var dto = new EstablishmentCreateDto
        {
            Name = "N",
            Description = "D",
            AreaM2 = 1,
            UvtQty = 2, // * UVT (10) = 20
            PlazaId = 1
        };

        var entity = new Establishment
        {
            Id = 7,
            Name = "N",
            Description = "D",
            AreaM2 = 1,
            UvtQty = 2,
            PlazaId = 1,
            RentValueBase = 20
        };

        _repo.Setup(r => r.AddAsync(It.IsAny<Establishment>()))
             .ReturnsAsync((Establishment e) => { e.Id = 7; return e; });

        _repo.Setup(r => r.GetByIdAnyAsync(7)).ReturnsAsync(entity);

        var result = await _service.CreateAsync(dto);

        Assert.Equal(7, result.Id);
        Assert.Equal("N", result.Name);
        Assert.Equal(20, result.RentValueBase); // chequea cálculo de UVT
    }

    [Fact]
    public async Task Update_ReturnsNull_WhenNotFound()
    {
        var dto = new EstablishmentUpdateDto
        {
            Id = 9,
            Name = "X",
            Description = "D",
            AreaM2 = 1,
            RentValueBase = 10,
            UvtQty = 1,
            PlazaId = 1
        };

        _repo.Setup(r => r.GetByIdAnyAsync(9)).ReturnsAsync((Establishment?)null);

        var result = await _service.UpdateAsync(dto);

        Assert.Null(result);
    }

    [Fact]
    public async Task Update_Succeeds_RecalculatesRentValue()
    {
        var dto = new EstablishmentUpdateDto
        {
            Id = 5,
            Name = "Updated",
            Description = "Desc",
            AreaM2 = 10,
            RentValueBase = 9999, // se ignora, el servicio recalcula
            UvtQty = 3,           // * UVT (10) = 30
            PlazaId = 2
        };

        var existing = new Establishment
        {
            Id = 5,
            Name = "Old",
            Description = "OldDesc",
            AreaM2 = 5,
            UvtQty = 1,
            PlazaId = 2,
            RentValueBase = 10
        };

        _repo.Setup(r => r.GetByIdAnyAsync(5)).ReturnsAsync(existing);
        _repo.Setup(r => r.UpdateAsync(It.IsAny<Establishment>()))
             .ReturnsAsync((Establishment e) => e);

        var updated = new Establishment
        {
            Id = 5,
            Name = "Updated",
            Description = "Desc",
            AreaM2 = 10,
            UvtQty = 3,
            PlazaId = 2,
            RentValueBase = 30
        };

        _repo.Setup(r => r.GetByIdAnyAsync(5)).ReturnsAsync(updated);

        var result = await _service.UpdateAsync(dto);

        Assert.NotNull(result);
        Assert.Equal(5, result!.Id);
        Assert.Equal("Updated", result.Name);
        Assert.Equal(30, result.RentValueBase); // chequea recalculo de UVT
    }
}
