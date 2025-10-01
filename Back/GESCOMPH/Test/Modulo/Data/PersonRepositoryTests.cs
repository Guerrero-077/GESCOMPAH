using Data.Repository;
using Entity.Domain.Models.Implements.Persons;
using Entity.Infrastructure.Context;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace Tests.Data.Persons;

public class PersonRepositoryTests
{
    private static ApplicationDbContext Ctx(string n)
    {
        var opt = new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase(n).Options;
        return new ApplicationDbContext(opt);
    }

    [Fact]
    public async Task Add_Get_Delete_Works()
    {
        var db = Guid.NewGuid().ToString();
        await using var ctx = Ctx(db);
        var repo = new DataGeneric<Person>(ctx);

        var p = await repo.AddAsync(new Person { FirstName = "A", LastName = "B" });
        (await repo.GetByIdAsync(p.Id)).Should().NotBeNull();
        (await repo.DeleteAsync(p.Id)).Should().BeTrue();
        (await repo.GetByIdAsync(p.Id)).Should().BeNull();
    }
}

