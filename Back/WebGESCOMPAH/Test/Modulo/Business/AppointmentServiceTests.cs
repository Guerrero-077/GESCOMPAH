using Business.Services.Business;
using Data.Interfaz.IDataImplement.Business;
using Data.Interfaz.IDataImplement.Persons;
using Entity.Domain.Models.Implements.Business;
using Entity.Domain.Models.Implements.Persons;
using Entity.DTOs.Implements.Business.Appointment;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using Moq;
using Utilities.Exceptions;

namespace Tests.Business.Business;

public class AppointmentServiceTests
{
    private readonly Mock<IAppointmentRepository> _repo = new();
    private readonly Mock<IMapper> _mapper = new();
    private readonly Mock<IPersonRepository> _persons = new();
    private readonly AppointmentService _service;

    public AppointmentServiceTests()
    {
        _service = new AppointmentService(_repo.Object, _mapper.Object, _persons.Object);
    }

    [Fact]
    public async Task RejectedAppointment_Throws_WhenIdZero()
    {
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => _service.RejectedAppointment(0));
        Assert.Contains("mayor que cero", ex.Message);
    }

    [Fact]
    public async Task Create_Throws_WhenDtoNull()
    {
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CreateAsync(null!));
        Assert.Contains("no puede ser nulo", ex.Message);
    }

    [Fact]
    public async Task Create_WithExistingPerson_MatchingData_CreatesAppointment()
    {
        var dto = new AppointmentCreateDto
        {
            FirstName = "A", LastName = "B", Document = "1", Address = "X", Phone = "Y", cityId = 1,
            Description = "d", RequestDate = DateTime.UtcNow, DateTimeAssigned = DateTime.UtcNow, EstablishmentId = 1
        };

        _persons.Setup(p => p.GetByDocumentAsync("1")).ReturnsAsync(new Person { Id = 10, FirstName = "A", LastName = "B", CityId = 1 });
        _repo.Setup(r => r.AddAsync(It.IsAny<Appointment>()))
             .ReturnsAsync((Appointment a) => { a.Id = 7; return a; });
        _mapper.Setup(m => m.Map<AppointmentSelectDto>(It.IsAny<Appointment>())).Returns(new AppointmentSelectDto { Id = 7 });

        var result = await _service.CreateAsync(dto);
        Assert.Equal(7, result.Id);
    }

    [Fact]
    public async Task Create_WithExistingPerson_MismatchedData_Throws()
    {
        var dto = new AppointmentCreateDto
        {
            FirstName = "A", LastName = "B", Document = "1", Address = "X", Phone = "Y", cityId = 1,
            Description = "d", RequestDate = DateTime.UtcNow, DateTimeAssigned = DateTime.UtcNow, EstablishmentId = 1
        };
        _persons.Setup(p => p.GetByDocumentAsync("1")).ReturnsAsync(new Person { Id = 10, FirstName = "AX", LastName = "B", CityId = 1 });
        await Assert.ThrowsAsync<BusinessException>(() => _service.CreateAsync(dto));
    }
}
