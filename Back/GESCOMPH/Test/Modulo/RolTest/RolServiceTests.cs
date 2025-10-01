using Business.Services.SecurityAuthentication;
using Data.Interfaz.DataBasic;
using Entity.DTOs.Implements.SecurityAuthentication.Rol;
using MapsterMapper;
using Moq;

// Alias para el modelo Rol del namespace específico
using RolEntity = Entity.Domain.Models.Implements.SecurityAuthentication.Rol;

namespace Test.Modulo.RolTest
{
    public class RolServiceTests
    {
        private readonly Mock<IDataGeneric<RolEntity>> _rolRepoMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly RolService _rolService;

        public RolServiceTests()
        {
            _rolRepoMock = new Mock<IDataGeneric<RolEntity>>();
            _mapperMock = new Mock<IMapper>();
            _rolService = new RolService(_rolRepoMock.Object, _mapperMock.Object);
        }

        #region GET ALL

        [Fact]
        public async Task GetAllAsync_ShouldReturnMappedDtos_WhenEntitiesExist()
        {
            var roles = new List<RolEntity>
            {
                new RolEntity { Id = 1, Name = "Admin" },
                new RolEntity { Id = 2, Name = "User" }
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

        #endregion

        #region GET BY ID

        [Fact]
        public async Task GetByIdAsync_ShouldReturnMappedDto_WhenEntityExists()
        {
            var rol = new RolEntity { Id = 1, Name = "Admin" };
            var dto = new RolSelectDto { Id = 1, Name = "Admin" };

            _rolRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(rol);
            _mapperMock.Setup(m => m.Map<RolSelectDto>(rol)).Returns(dto);

            var result = await _rolService.GetByIdAsync(1);

            Assert.NotNull(result);
            Assert.Equal("Admin", result!.Name);
        }

        #endregion

        #region CREATE

        [Fact]
        public async Task CreateAsync_ShouldReturnDto_WhenCreatedSuccessfully()
        {
            var dto = new RolCreateDto { Name = "Admin" };
            var entity = new RolEntity { Id = 1, Name = "Admin" };
            var mappedDto = new RolSelectDto { Id = 1, Name = "Admin" };

            _mapperMock.Setup(m => m.Map<RolEntity>(dto)).Returns(entity);
            _rolRepoMock.Setup(r => r.GetAllQueryable()).Returns(new List<RolEntity>().AsQueryable());
            _rolRepoMock.Setup(r => r.AddAsync(entity)).ReturnsAsync(entity);
            _mapperMock.Setup(m => m.Map<RolSelectDto>(entity)).Returns(mappedDto);

            var result = await _rolService.CreateAsync(dto);

            Assert.NotNull(result);
            Assert.Equal("Admin", result.Name);
        }

        #endregion

        #region UPDATE

        [Fact]
        public async Task UpdateAsync_ShouldReturnDto_WhenUpdateSucceeds()
        {
            var dto = new RolUpdateDto { Id = 1, Name = "Updated" };
            var entity = new RolEntity { Id = 1, Name = "Updated" };
            var mappedDto = new RolSelectDto { Id = 1, Name = "Updated" };

            _mapperMock.Setup(m => m.Map<RolEntity>(dto)).Returns(entity);
            _rolRepoMock.Setup(r => r.UpdateAsync(entity)).ReturnsAsync(entity);
            _mapperMock.Setup(m => m.Map<RolSelectDto>(entity)).Returns(mappedDto);

            var result = await _rolService.UpdateAsync(dto);

            Assert.NotNull(result);
            Assert.Equal("Updated", result.Name);
        }

        #endregion

        #region DELETE

        [Fact]
        public async Task DeleteAsync_ShouldReturnTrue_WhenEntityDeleted()
        {
            _rolRepoMock.Setup(r => r.DeleteAsync(1)).ReturnsAsync(true);

            var result = await _rolService.DeleteAsync(1);

            Assert.True(result);
            _rolRepoMock.Verify(r => r.DeleteAsync(1), Times.Once);
        }

        #endregion

        #region DELETE LOGIC

        [Fact]
        public async Task DeleteLogicAsync_ShouldReturnTrue_WhenEntityDeletedLogically()
        {
            _rolRepoMock.Setup(r => r.DeleteLogicAsync(1)).ReturnsAsync(true);

            var result = await _rolService.DeleteLogicAsync(1);

            Assert.True(result);
            _rolRepoMock.Verify(r => r.DeleteLogicAsync(1), Times.Once);
        }

        #endregion

        #region UPDATE ACTIVE STATUS

        [Fact]
        public async Task UpdateActiveStatusAsync_ShouldUpdateEntity_WhenIdIsValid()
        {
            var entity = new RolEntity { Id = 1, Name = "Admin", Active = false };

            _rolRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(entity);
            _rolRepoMock.Setup(r => r.UpdateAsync(entity)).ReturnsAsync(entity);

            await _rolService.UpdateActiveStatusAsync(1, true);

            _rolRepoMock.Verify(r => r.UpdateAsync(It.Is<RolEntity>(x => x.Id == 1 && x.Active == true)), Times.Once);
        }

        #endregion
    }
}
