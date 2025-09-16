using Data.Repository;
using Entity.Domain.Models.Implements.Location;
using Entity.Infrastructure.Context;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace Tests.Data.Locations;

public class DepartmentRepositoryTests
{
    private static ApplicationDbContext Ctx(string name)
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(name)
            .Options;
        return new ApplicationDbContext(options);
    }

    [Fact]
    public async Task Crud_WithSoftDelete_Works()
    {
        var db = Guid.NewGuid().ToString();
        await using var ctx = Ctx(db);
        var repo = new DataGeneric<Department>(ctx);

        var d = await repo.AddAsync(new Department { Name = "Dept" });
        (await repo.GetByIdAsync(d.Id)).Should().NotBeNull();
        (await repo.DeleteLogicAsync(d.Id)).Should().BeTrue();
        (await repo.GetByIdAsync(d.Id)).Should().BeNull();
    }
}

