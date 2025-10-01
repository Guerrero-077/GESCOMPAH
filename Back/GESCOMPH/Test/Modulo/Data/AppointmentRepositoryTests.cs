using Data.Services.Business;
using Entity.Domain.Models.Implements.Business;
using Entity.Domain.Models.Implements.Persons;
using Entity.Infrastructure.Context;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace Tests.Data.Business;

public class AppointmentRepositoryTests
{
    private static ApplicationDbContext Ctx()
    {
        var opt = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new ApplicationDbContext(opt);
    }

}

