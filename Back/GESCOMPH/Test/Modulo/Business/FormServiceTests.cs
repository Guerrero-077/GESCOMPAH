using Business.Services.AdministrationSystem;
using Data.Interfaz.DataBasic;
using Entity.Domain.Models.Implements.AdministrationSystem;
using Entity.DTOs.Implements.AdministrationSystem.Form;
using MapsterMapper;
using Moq;
using Utilities.Exceptions;

namespace Test.Modulo.Business;

public class FormServiceTests
{
    private readonly Mock<IDataGeneric<Form>> _repo = new();
    private readonly Mock<IMapper> _mapper = new();
    private readonly FormService _service;

    public FormServiceTests()
    {
        _service = new FormService(_repo.Object, _mapper.Object);
    }

    [Fact]
    public async Task Create_Throws_WhenActiveDuplicateExists()
    {
        var dto = new FormCreateDto { Name = "Menu", Description = "D" };
        var candidate = new Form { Name = "Menu", Description = "D", IsDeleted = false };
        _mapper.Setup(m => m.Map<Form>(dto)).Returns(candidate);
        _repo.Setup(r => r.GetAllQueryable()).Returns(new List<Form> { new() { Id = 1, Name = "Menu", IsDeleted = false } }.AsQueryable());
        await Assert.ThrowsAsync<BusinessException>(() => _service.CreateAsync(dto));
    }

    [Fact]
    public async Task Create_Reactivates_WhenInactiveDuplicateExists()
    {
        var dto = new FormCreateDto { Name = "Menu", Description = "D" };
        var candidate = new Form { Name = "Menu", Description = "D" };
        _mapper.Setup(m => m.Map<Form>(dto)).Returns(candidate);
        _repo.Setup(r => r.GetAllQueryable()).Returns(new List<Form> { new() { Id = 10, Name = "Menu", IsDeleted = true } }.AsQueryable());

        _repo.Setup(r => r.UpdateAsync(It.Is<Form>(f => f.Id == 10))).ReturnsAsync(new Form { Id = 10, Name = "Menu" });
        _mapper.Setup(m => m.Map<FormSelectDto>(It.IsAny<Form>())).Returns(new FormSelectDto { Id = 10, Name = "Menu" });

        var result = await _service.CreateAsync(dto);
        Assert.Equal(10, result.Id);
        _repo.Verify(r => r.UpdateAsync(It.Is<Form>(f => f.Id == 10 && f.IsDeleted == false)), Times.Once);
    }
}

