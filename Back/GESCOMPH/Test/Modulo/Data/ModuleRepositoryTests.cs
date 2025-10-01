using Data.Repository;
using Entity.Domain.Models.Implements.AdministrationSystem;
using Entity.Infrastructure.Context;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace Tests.Data.AdministrationSystem;

public class ModuleRepositoryTests
{
    private static ApplicationDbContext Ctx(string n)
    {
        var opt = new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase(n).Options;
        return new ApplicationDbContext(opt);
    }

    [Fact]
    public async Task Add_GetAll_Delete_Works()
    {
        var db = Guid.NewGuid().ToString();
        await using var ctx = Ctx(db);
        var repo = new DataGeneric<Module>(ctx);

        var m = await repo.AddAsync(new Module { Name = "M", Description = "D", Icon = "mdi-home" });
        (await repo.GetAllAsync()).Should().ContainSingle(x => x.Id == m.Id);
        (await repo.DeleteAsync(m.Id)).Should().BeTrue();
        (await repo.GetByIdAsync(m.Id)).Should().BeNull();
    }
}
