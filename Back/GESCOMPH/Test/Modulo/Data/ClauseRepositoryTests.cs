using Data.Repository;
using Entity.Domain.Models.Implements.Business;
using Entity.Infrastructure.Context;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace Test.Modulo.Data;

public class ClauseRepositoryTests
{
    private static ApplicationDbContext Ctx(string n)
    {
        var opt = new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase(n).Options;
        return new ApplicationDbContext(opt);
    }

    [Fact]
    public async Task Add_Update_SoftDelete_Works()
    {
        var db = Guid.NewGuid().ToString();
        await using var ctx = Ctx(db);
        var repo = new DataGeneric<Clause>(ctx);

        var created = await repo.AddAsync(new Clause { Name = "N", Description = "D" });
        created.Id.Should().BeGreaterThan(0);

        created.Description = "D2";
        var updated = await repo.UpdateAsync(created);
        updated.Description.Should().Be("D2");

        (await repo.DeleteLogicAsync(updated.Id)).Should().BeTrue();
        (await repo.GetByIdAsync(updated.Id)).Should().BeNull();
    }
}

