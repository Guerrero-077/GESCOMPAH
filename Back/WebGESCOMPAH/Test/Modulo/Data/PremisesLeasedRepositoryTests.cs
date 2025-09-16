using Data.Services.Business;
using Entity.Domain.Models.Implements.Business;
using Entity.Infrastructure.Context;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace Tests.Data.Business;

public class PremisesLeasedRepositoryTests
{
    private static ApplicationDbContext Ctx()
    {
        var opt = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new ApplicationDbContext(opt);
    }

    [Fact]
    public async Task Crud_AndAddRange_Works()
    {
        await using var ctx = Ctx();
        var repo = new PremisesLeasedRepository(ctx);

        // Seed establishment and contract minimal
        ctx.contracts.Add(new Contract { Id = 1, PersonId = 1, StartDate = DateTime.UtcNow, EndDate = DateTime.UtcNow });
        ctx.Establishment.Add(new Establishment { Id = 2, Name = "E", Description = "" });
        await ctx.SaveChangesAsync();

        var p = await repo.AddAsync(new PremisesLeased { ContractId = 1, EstablishmentId = 2 });
        p.Id.Should().BeGreaterThan(0);

        var list = await repo.GetAllAsync();
        list.Should().NotBeEmpty();

        var byId = await repo.GetByIdAsync(p.Id);
        byId.Should().NotBeNull();

        // Update
        await repo.UpdateAsync(new PremisesLeased { Id = p.Id, ContractId = 1, EstablishmentId = 2 });
        (await repo.GetByIdAsync(p.Id)).Should().NotBeNull();

        // AddRange
        await repo.AddRangeAsync(new [] { new PremisesLeased { ContractId = 1, EstablishmentId = 2 } });
        (await ctx.premisesLeaseds.CountAsync()).Should().BeGreaterThan(1);

        // Delete logic
        (await repo.DeleteLogicAsync(p.Id)).Should().BeTrue();
        (await repo.GetByIdAsync(p.Id)).Should().BeNull();

        // Delete physical of another
        var second = await ctx.premisesLeaseds.OrderByDescending(x => x.Id).FirstAsync();
        (await repo.DeleteAsync(second.Id)).Should().BeTrue();
    }
}

