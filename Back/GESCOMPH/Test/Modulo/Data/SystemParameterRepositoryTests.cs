using Data.Repository;
using Entity.Domain.Models.Implements.AdministrationSystem;
using Entity.Infrastructure.Context;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace Test.Modulo.Data;

public class SystemParameterRepositoryTests
{
    private static ApplicationDbContext Ctx(string n)
    {
        var opt = new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase(n).Options;
        return new ApplicationDbContext(opt);
    }

    [Fact]
    public async Task Add_GetAll_Update_DeleteLogic_Works()
    {
        var db = Guid.NewGuid().ToString();
        await using var ctx = Ctx(db);
        var repo = new DataGeneric<SystemParameter>(ctx);

        var p = await repo.AddAsync(new SystemParameter { Key = "UVT", Value = "49798.75", EffectiveFrom = DateTime.UtcNow });
        (await repo.GetAllAsync()).Should().ContainSingle(x => x.Id == p.Id);

        p.Value = "50000";
        await repo.UpdateAsync(p);
        var again = await repo.GetByIdAsync(p.Id);
        again!.Value.Should().Be("50000");

        (await repo.DeleteLogicAsync(p.Id)).Should().BeTrue();
        (await repo.GetByIdAsync(p.Id)).Should().BeNull();
    }
}

