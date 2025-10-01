using Business.Services.SecurityAuthentication;
using Data.Interfaz.DataBasic;
using Entity.Domain.Models.Implements.SecurityAuthentication;
using Entity.DTOs.Implements.SecurityAuthentication.Permission;
using MapsterMapper;
using Moq;
using Utilities.Exceptions;

namespace Tests.Business.SecurityAuthentication;

public class PermissionServiceTests
{
    private readonly Mock<IDataGeneric<Permission>> _repo = new();
    private readonly Mock<IMapper> _mapper = new();
    private readonly PermissionService _service;

    public PermissionServiceTests()
    {
        _service = new PermissionService(_repo.Object, _mapper.Object);
    }

    [Fact]
    public async Task GetAll_WrapsExceptions_AsBusinessException()
    {
        _repo.Setup(r => r.GetAllAsync()).ThrowsAsync(new Exception("db"));
        var ex = await Assert.ThrowsAsync<BusinessException>(() => _service.GetAllAsync());
        Assert.Contains("Error al obtener todos los registros", ex.Message);
    }
}

