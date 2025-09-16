using Business.Services.SecurityAuthentication;
using Data.Interfaz.DataBasic;
using Entity.Domain.Models.Implements.SecurityAuthentication;
using Entity.DTOs.Implements.SecurityAuthentication.Rol;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using Moq;
using Utilities.Exceptions;

namespace Tests.Business.SecurityAuthentication
{
    public class RolServiceTests
    {
        private readonly Mock<IDataGeneric<Rol>> _rolRepoMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly RolService _rolService;

        public RolServiceTests()
        {
            _rolRepoMock = new Mock<IDataGeneric<Rol>>();
            _mapperMock = new Mock<IMapper>();
            _rolService = new RolService(_rolRepoMock.Object, _mapperMock.Object);
        }

        // ---------- GETALL ----------
        [Fact]
        public async Task GetAllAsync()
        {
            var roles = new List<Rol>
            {
                new Rol { Id = 1, Name = "Admin" },
                new Rol { Id = 2, Name = "User" }
            };

            _rolRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(roles);
            _mapperMock.Setup(m => m.Map<IEnumerable<RolSelectDto>>(roles))
                       .Returns(new List<RolSelectDto> {
                           new RolSelectDto { Id = 1, Name = "Admin" },
                           new RolSelectDto { Id = 2, Name = "User" }
                       });

            var result = await _rolService.GetAllAsync();

            Assert.NotNull(result);
            Assert.Collection(result,
                r => Assert.Equal("Admin", r.Name),
                r => Assert.Equal("User", r.Name));
        }

        [Fact]
        public async Task GetAllAsync_ShouldThrowBusinessException_WhenRepoFails()
        {
            _rolRepoMock.Setup(r => r.GetAllAsync()).ThrowsAsync(new Exception("DB failure"));

            var ex = await Assert.ThrowsAsync<BusinessException>(() => _rolService.GetAllAsync());

            Assert.Contains("Error al obtener todos los registros", ex.Message);
            Assert.Equal("DB failure", ex.InnerException!.Message);
        }

        // ---------- GETBYID ----------
        [Fact]
        public async Task GetByIdAsync()
        {
            var rol = new Rol { Id = 1, Name = "Admin" };

            _rolRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(rol);
            _mapperMock.Setup(m => m.Map<RolSelectDto>(rol))
                       .Returns(new RolSelectDto { Id = 1, Name = "Admin" });

            var result = await _rolService.GetByIdAsync(1);

            Assert.NotNull(result);
            Assert.Equal("Admin", result!.Name);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldThrowBusinessException_WhenIdIsZero()
        {
            var ex = await Assert.ThrowsAsync<BusinessException>(() => _rolService.GetByIdAsync(0));

            Assert.Contains("Error al obtener el registro con ID 0", ex.Message);
            Assert.Contains("El ID debe ser mayor que cero", ex.InnerException!.Message);
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsNull_WhenEntityNotFound()
        {
            _rolRepoMock.Setup(r => r.GetByIdAsync(10)).ReturnsAsync((Rol?)null);

            var result = await _rolService.GetByIdAsync(10);

            Assert.Null(result);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldThrowBusinessException_WhenRepoFails()
        {
            _rolRepoMock.Setup(r => r.GetByIdAsync(5)).ThrowsAsync(new Exception("DB broken"));

            var ex = await Assert.ThrowsAsync<BusinessException>(() => _rolService.GetByIdAsync(5));

            Assert.Contains("Error al obtener el registro con ID 5", ex.Message);
            Assert.Equal("DB broken", ex.InnerException!.Message);
        }

        // ---------- CREATE ----------
        [Fact]
        public async Task CreateAsync()
        {
            var dto = new RolCreateDto { Name = "Admin" };
            var candidate = new Rol { Name = "Admin" };

            _mapperMock.Setup(m => m.Map<Rol>(dto)).Returns(candidate);
            _rolRepoMock.Setup(r => r.GetAllQueryable()).Returns(new List<Rol>().AsQueryable());

            await _rolService.CreateAsync(dto);

            _rolRepoMock.Verify(r => r.AddAsync(It.Is<Rol>(x => x.Name == "Admin")), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_ShouldThrowBusinessException_WhenDuplicateExists()
        {
            var dto = new RolCreateDto { Name = "Admin" };
            var candidate = new Rol { Name = "Admin" };

            _mapperMock.Setup(m => m.Map<Rol>(dto)).Returns(candidate);

            var existing = new List<Rol>
            {
                new Rol { Id = 1, Name = "Admin", IsDeleted = false }
            }.AsQueryable();

            _rolRepoMock.Setup(r => r.GetAllQueryable()).Returns(existing);

            var ex = await Assert.ThrowsAsync<BusinessException>(() => _rolService.CreateAsync(dto));

            Assert.Contains("Error al crear el registro", ex.Message);
            Assert.Contains("Ya existe un registro con los mismos datos", ex.InnerException!.Message);
        }

        [Fact]
        public async Task CreateAsync_ShouldReactivate_WhenDuplicateInactiveExists()
        {
            var dto = new RolCreateDto { Name = "Admin" };
            var candidate = new Rol { Name = "Admin" };

            _mapperMock.Setup(m => m.Map<Rol>(dto)).Returns(candidate);

            var existing = new List<Rol>
            {
                new Rol { Id = 1, Name = "Admin", IsDeleted = true }
            }.AsQueryable();

            _rolRepoMock.Setup(r => r.GetAllQueryable()).Returns(existing);

            await _rolService.CreateAsync(dto);

            _rolRepoMock.Verify(r => r.UpdateAsync(It.Is<Rol>(x => x.Id == 1 && x.IsDeleted == false)), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_ShouldThrowBusinessException_WhenDtoIsNull()
        {
            await Assert.ThrowsAsync<BusinessException>(() => _rolService.CreateAsync(null!));
        }

        [Fact]
        public async Task CreateAsync_ShouldThrowBusinessException_WhenDbUpdateExceptionOccurs()
        {
            var dto = new RolCreateDto { Name = "Admin" };
            var candidate = new Rol { Name = "Admin" };

            _mapperMock.Setup(m => m.Map<Rol>(dto)).Returns(candidate);
            _rolRepoMock.Setup(r => r.GetAllQueryable()).Returns(new List<Rol>().AsQueryable());
            _rolRepoMock.Setup(r => r.AddAsync(It.IsAny<Rol>()))
                        .ThrowsAsync(new DbUpdateException("Unique violation"));

            var ex = await Assert.ThrowsAsync<BusinessException>(() => _rolService.CreateAsync(dto));

            Assert.Contains("Violación de unicidad", ex.Message);
        }

        [Fact]
        public async Task CreateAsync_ShouldThrowBusinessException_WhenGenericErrorOccurs()
        {
            var dto = new RolCreateDto { Name = "Admin" };
            var candidate = new Rol { Name = "Admin" };

            _mapperMock.Setup(m => m.Map<Rol>(dto)).Returns(candidate);
            _rolRepoMock.Setup(r => r.GetAllQueryable()).Returns(new List<Rol>().AsQueryable());
            _rolRepoMock.Setup(r => r.AddAsync(It.IsAny<Rol>()))
                        .ThrowsAsync(new Exception("Unexpected error"));

            var ex = await Assert.ThrowsAsync<BusinessException>(() => _rolService.CreateAsync(dto));

            Assert.Contains("Error al crear el registro", ex.Message);
            Assert.Equal("Unexpected error", ex.InnerException!.Message);
        }

        // ---------- UPDATE ----------
        [Fact]
        public async Task UpdateAsync()
        {
            var dto = new RolUpdateDto { Id = 1, Name = "Updated" };
            _mapperMock.Setup(m => m.Map<Rol>(dto)).Returns(new Rol { Id = 1, Name = "Updated" });

            await _rolService.UpdateAsync(dto);

            _rolRepoMock.Verify(r => r.UpdateAsync(It.Is<Rol>(x => x.Name == "Updated")), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_ShouldThrowBusinessException_WhenDtoIsNull()
        {
            await Assert.ThrowsAsync<BusinessException>(() => _rolService.UpdateAsync(null!));
        }

        // ---------- DELETE ----------
        [Fact]
        public async Task DeleteAsync()
        {
            _rolRepoMock.Setup(r => r.DeleteLogicAsync(1)).ReturnsAsync(true);

            await _rolService.DeleteAsync(1);

            _rolRepoMock.Verify(r => r.DeleteAsync(1), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_ShouldThrowBusinessException_WhenIdIsZero()
        {
            var ex = await Assert.ThrowsAsync<BusinessException>(() => _rolService.DeleteAsync(0));

            Assert.Contains("El ID debe ser mayor que cero", ex.InnerException!.Message);
        }

        [Fact]
        public async Task DeleteAsync_ShouldThrowBusinessException_WhenDbUpdateExceptionOccurs()
        {
            _rolRepoMock.Setup(r => r.DeleteAsync(1))
                        .ThrowsAsync(new DbUpdateException("FK constraint"));

            var ex = await Assert.ThrowsAsync<BusinessException>(() => _rolService.DeleteAsync(1));

            Assert.Contains("restricciones de datos", ex.Message);
        }

        [Fact]
        public async Task DeleteAsync_ShouldThrowBusinessException_WhenGenericErrorOccurs()
        {
            _rolRepoMock.Setup(r => r.DeleteAsync(1))
                        .ThrowsAsync(new Exception("Delete fail"));

            var ex = await Assert.ThrowsAsync<BusinessException>(() => _rolService.DeleteAsync(1));

            Assert.Contains("Error al eliminar el registro", ex.Message);
            Assert.Equal("Delete fail", ex.InnerException!.Message);
        }

        // ---------- DELETE LOGIC ----------
        [Fact]
        public async Task DeleteLogicAsync()
        {
            _rolRepoMock.Setup(r => r.DeleteLogicAsync(1)).ReturnsAsync(true);

            await _rolService.DeleteLogicAsync(1);

            _rolRepoMock.Verify(r => r.DeleteLogicAsync(1), Times.Once);
        }

        [Fact]
        public async Task DeleteLogicAsync_ShouldThrowBusinessException_WhenIdIsZero()
        {
            var ex = await Assert.ThrowsAsync<BusinessException>(() => _rolService.DeleteLogicAsync(0));

            Assert.Contains("El ID debe ser mayor que cero", ex.InnerException!.Message);
        }

        [Fact]
        public async Task DeleteLogicAsync_ShouldThrowBusinessException_WhenGenericErrorOccurs()
        {
            _rolRepoMock.Setup(r => r.DeleteLogicAsync(1))
                        .ThrowsAsync(new Exception("Logic delete fail"));

            var ex = await Assert.ThrowsAsync<BusinessException>(() => _rolService.DeleteLogicAsync(1));

            Assert.Contains("Error al eliminar lógicamente", ex.Message);
            Assert.Equal("Logic delete fail", ex.InnerException!.Message);
        }

        // ---------- UPDATE ACTIVE STATUS ----------
        [Fact]
        public async Task UpdateActiveStatusAsync()
        {
            var entity = new Rol { Id = 1, Name = "Admin", Active = false };

            _rolRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(entity);

            await _rolService.UpdateActiveStatusAsync(1, true);

            _rolRepoMock.Verify(r => r.UpdateAsync(It.Is<Rol>(x => x.Id == 1 && x.Active == true)), Times.Once);
        }

        [Fact]
        public async Task UpdateActiveStatusAsync_ShouldThrowBusinessException_WhenIdIsZero()
        {
            var ex = await Assert.ThrowsAsync<BusinessException>(() => _rolService.UpdateActiveStatusAsync(0, true));

            Assert.Contains("El ID debe ser mayor que cero", ex.InnerException!.Message);
        }

        [Fact]
        public async Task UpdateActiveStatusAsync_ShouldThrowBusinessException_WhenEntityDoesNotExist()
        {
            _rolRepoMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Rol?)null);

            var ex = await Assert.ThrowsAsync<BusinessException>(() => _rolService.UpdateActiveStatusAsync(99, true));

            Assert.Contains("Error al actualizar el estado del registro con ID 99", ex.Message);
            Assert.IsType<KeyNotFoundException>(ex.InnerException);
            Assert.Contains("No se encontró el registro con ID 99", ex.InnerException!.Message);
        }
    }
}
