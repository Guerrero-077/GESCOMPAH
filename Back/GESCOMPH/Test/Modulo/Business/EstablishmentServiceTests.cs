using Business.Services.Business;
using Data.Interfaz.IDataImplement.Business;
using Entity.Domain.Models.Implements.Business;
using Entity.DTOs.Implements.Business.EstablishmentDto;
using Entity.Infrastructure.Context;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Utilities.Exceptions;

namespace Test.Modulo.Business;

public class EstablishmentServiceTests
{
    private readonly Mock<IEstablishmentsRepository> _repo = new();
    private readonly Mock<IMapper> _mapper = new();
    private readonly Mock<ILogger<EstablishmentService>> _logger = new();
    private readonly ApplicationDbContext _ctx;
    private readonly EstablishmentService _service;

    public EstablishmentServiceTests()
    {
        var opt = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        _ctx = new ApplicationDbContext(opt);
        _service = new EstablishmentService(_repo.Object, _ctx, _mapper.Object, _logger.Object);
    }

    [Fact]
    public async Task Create_Throws_WhenInvalidValues()
    {
        var dto = new EstablishmentCreateDto { AreaM2 = 0, RentValueBase = 0, UvtQty = 0, PlazaId = 0 };
        await Assert.ThrowsAsync<BusinessException>(() => _service.CreateAsync(dto));
    }

    [Fact]
    public async Task Create_Succeeds_ReturnsSelect()
    {
        var dto = new EstablishmentCreateDto { Name = "N", Description = "D", AreaM2 = 1, RentValueBase = 10, UvtQty = 1, PlazaId = 1 };
        var entity = new Establishment { Id = 7, Name = "N", Description = "D", AreaM2 = 1, RentValueBase = 10, UvtQty = 1, PlazaId = 1 };
        _repo.Setup(r => r.AddAsync(It.IsAny<Establishment>()))
             .ReturnsAsync((Establishment e) => { e.Id = 7; return e; });
        _repo.Setup(r => r.GetByIdAnyAsync(7)).ReturnsAsync(entity);

        var result = await _service.CreateAsync(dto);
        Assert.Equal(7, result.Id);
        Assert.Equal("N", result.Name);
    }

    [Fact]
    public async Task Update_ReturnsNull_WhenNotFound()
    {
        var dto = new EstablishmentUpdateDto { Id = 9, Name = "X", Description = "D", AreaM2 = 1, RentValueBase = 10, UvtQty = 1, PlazaId = 1 };
        _repo.Setup(r => r.GetByIdAnyAsync(9)).ReturnsAsync((Establishment?)null);
        var result = await _service.UpdateAsync(dto);
        Assert.Null(result);
    }
}
