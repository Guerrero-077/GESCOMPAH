using Data.Repository;
using Entity.Domain.Models.Implements.SecurityAuthentication;
using Entity.Infrastructure.Context;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace Tests.Data.SecurityAuthentication;

public class PermissionRepositoryTests
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
        var repo = new DataGeneric<Permission>(ctx);

        var p = await repo.AddAsync(new Permission { Name = "P", Description = "D" });
        (await repo.GetAllAsync()).Should().ContainSingle(x => x.Id == p.Id);

        p.Description = "D2";
        await repo.UpdateAsync(p);
        var again = await repo.GetByIdAsync(p.Id);
        again!.Description.Should().Be("D2");

        (await repo.DeleteLogicAsync(p.Id)).Should().BeTrue();
        (await repo.GetByIdAsync(p.Id)).Should().BeNull();
    }
}

