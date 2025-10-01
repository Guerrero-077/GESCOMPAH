using Data.Services.Business;
using Entity.Domain.Models.Implements.Business;
using Entity.Domain.Models.Implements.Persons;
using Entity.Infrastructure.Context;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace Test.Modulo.Data;

public class AppointmentRepositoryTests
{
    private static ApplicationDbContext Ctx()
    {
        var opt = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new ApplicationDbContext(opt);
    }

    [Fact]
    public async Task RejectedAppointment_ReturnsTrue_WhenExists()
    {
        await using var ctx = Ctx();
        var repo = new AppointmentRepository(ctx);

        // Seed minimal required related entities
        ctx.Persons.Add(new Person { Id = 1, FirstName = "A", LastName = "B", CityId = 1 });
        ctx.Establishment.Add(new Establishment { Id = 1, Name = "E", Description = "", Active = true });
        ctx.Appointments.Add(new Appointment { Id = 5, Description = "d", RequestDate = DateTime.UtcNow, PersonId = 1, EstablishmentId = 1, Status = 0, Active = true });
        await ctx.SaveChangesAsync();

        var ok = await repo.RejectedAppointment(5);
        ok.Should().BeTrue();
        (await ctx.Appointments.FindAsync(5))!.Status.Should().Be(3);
    }

    [Fact]
    public async Task RejectedAppointment_Throws_WhenNotExists()
    {
        await using var ctx = Ctx();
        var repo = new AppointmentRepository(ctx);
        await Assert.ThrowsAsync<Exception>(() => repo.RejectedAppointment(999));
    }
}

