using Data.Services.AdministratiosSystem;
using Entity.Domain.Models.Implements.AdministrationSystem;
using Entity.Infrastructure.Context;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace Test.Modulo.Data;

public class FormModuleRepositoryTests
{
    private static ApplicationDbContext Ctx()
    {
        var opt = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new ApplicationDbContext(opt);
    }

    [Fact]
    public async Task Crud_Works()
    {
        await using var ctx = Ctx();
        var repo = new FormModuleRepository(ctx);
        ctx.Modules.Add(new Module { Id = 1, Name = "M", Description = "D", Icon = "i" });
        ctx.Forms.Add(new Form { Id = 2, Name = "F", Description = "D", Route = "/f" });
        await ctx.SaveChangesAsync();

        var fm = await repo.AddAsync(new FormModule { FormId = 2, ModuleId = 1 });
        fm.Id.Should().BeGreaterThan(0);

        var byId = await repo.GetByIdAsync(fm.Id);
        byId.Should().NotBeNull();

        await repo.UpdateAsync(new FormModule { Id = fm.Id, FormId = 2, ModuleId = 1 });
        (await repo.GetByIdAsync(fm.Id)).Should().NotBeNull();

        (await repo.DeleteLogicAsync(fm.Id)).Should().BeTrue();
        (await repo.GetByIdAsync(fm.Id)).Should().BeNull();
    }
}

