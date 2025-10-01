using Business.Services.Persons;
using Data.Interfaz.IDataImplement.Persons;
using Entity.Domain.Models.Implements.Persons;
using Entity.DTOs.Implements.Persons.Person;
using MapsterMapper;
using Moq;
using Utilities.Exceptions;

namespace Tests.Business.Persons;

public class PersonServiceTests
{
    private readonly Mock<IPersonRepository> _personRepo = new();
    private readonly Mock<IMapper> _mapper = new();
    private readonly PersonService _service;

    public PersonServiceTests()
    {
        _service = new PersonService(_personRepo.Object, _mapper.Object);
    }

    [Fact]
    public async Task Create_Throws_WhenDuplicateDocument()
    {
        var dto = new PersonDto { Document = "123" };
        _personRepo.Setup(r => r.ExistsByDocumentAsync("123")).ReturnsAsync(true);
        await Assert.ThrowsAsync<BusinessException>(() => _service.CreateAsync(dto));
    }

    [Fact]
    public async Task Update_Throws_WhenNotFound()
    {
        var dto = new PersonUpdateDto { Id = 5, FirstName = "A", LastName = "B", Address = "", Phone = "", CityId = 1 };
        _personRepo.Setup(r => r.GetByIdAsync(5)).ReturnsAsync((Person?)null);
        await Assert.ThrowsAsync<BusinessException>(() => _service.UpdateAsync(dto));
    }

    [Fact]
    public async Task Create_Succeeds_AndMapsSelect()
    {
        var dto = new PersonDto { Document = "123", FirstName = "A", LastName = "B", CityId = 1 };
        _personRepo.Setup(r => r.ExistsByDocumentAsync("123")).ReturnsAsync(false);

        var createdEntity = new Person { Id = 10, FirstName = "A", LastName = "B", CityId = 1 };
        _mapper.Setup(m => m.Map<Person>(dto)).Returns(createdEntity);
        _personRepo.Setup(r => r.AddAsync(createdEntity)).ReturnsAsync(createdEntity);
        _personRepo.Setup(r => r.GetByIdAsync(10)).ReturnsAsync(createdEntity);
        _mapper.Setup(m => m.Map<PersonSelectDto>(createdEntity)).Returns(new PersonSelectDto { Id = 10, FirstName = "A", LastName = "B", CityId = 1, CityName = "X" });

        var result = await _service.CreateAsync(dto);
        Assert.Equal(10, result.Id);
        Assert.Equal("A", result.FirstName);
    }

    [Fact]
    public async Task Update_Succeeds_AndMapsSelect()
    {
        var dto = new PersonUpdateDto { Id = 5, FirstName = "A2", LastName = "B2", Address = "Q", Phone = "P", CityId = 2 };
        var existing = new Person { Id = 5, FirstName = "A", LastName = "B", CityId = 1 };
        _personRepo.Setup(r => r.GetByIdAsync(5)).ReturnsAsync(existing);

        _mapper.Setup(m => m.Map(dto, existing)).Returns(existing);
        _personRepo.Setup(r => r.UpdateAsync(existing)).ReturnsAsync(existing);
        _personRepo.Setup(r => r.GetByIdAsync(5)).ReturnsAsync(existing);
        _mapper.Setup(m => m.Map<PersonSelectDto>(existing)).Returns(new PersonSelectDto { Id = 5, FirstName = "A2", LastName = "B2", CityId = 2, CityName = "Y" });

        var result = await _service.UpdateAsync(dto);
        Assert.Equal(5, result.Id);
        Assert.Equal("A2", result.FirstName);
    }
}
