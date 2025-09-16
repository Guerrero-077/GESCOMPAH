using Data.Repository;
using Entity.Domain.Models.Implements.AdministrationSystem;
using Entity.Infrastructure.Context;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace Tests.Data.AdministrationSystem;

public class FormRepositoryTests
{
    private static ApplicationDbContext Ctx(string n)
    {
        var opt = new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase(n).Options;
        return new ApplicationDbContext(opt);
    }

    [Fact]
    public async Task Add_Update_DeleteLogic_Works()
    {
        var db = Guid.NewGuid().ToString();
        await using var ctx = Ctx(db);
        var repo = new DataGeneric<Form>(ctx);

        var f = await repo.AddAsync(new Form { Name = "Menu", Description = "D", Route = "/home" });
        (await repo.GetByIdAsync(f.Id)).Should().NotBeNull();

        f.Description = "D2";
        var updated = await repo.UpdateAsync(f);
        updated.Description.Should().Be("D2");

        (await repo.DeleteLogicAsync(f.Id)).Should().BeTrue();
        (await repo.GetByIdAsync(f.Id)).Should().BeNull();
    }
}

