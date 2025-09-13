using Data.Services.Business;
using Entity.Domain.Models.Implements.Business;
using Entity.Infrastructure.Context;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace Tests.Data.Business;

public class EstablishmentsRepositoryTests
{
    private static ApplicationDbContext Ctx()
    {
        var opt = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new ApplicationDbContext(opt);
    }

    [Fact]
    public async Task GetAllAsync_FiltersByActiveOnly()
    {
        await using var ctx = Ctx();
        var repo = new EstablishmentsRepository(ctx);

        var plaza = new Plaza { Id = 1, Name = "P", Description = "", Active = true, Location = "L" };
        ctx.plaza.Add(plaza);
        ctx.Establishment.AddRange(
            new Establishment { Id = 1, Name = "A", Description = "", Active = true, PlazaId = 1, Plaza = plaza },
            new Establishment { Id = 2, Name = "B", Description = "", Active = false, PlazaId = 1, Plaza = plaza }
        );
        await ctx.SaveChangesAsync();

        var all = await repo.GetAllAsync(global::Data.Interfaz.IDataImplement.Business.ActivityFilter.Any);
        all.Should().HaveCount(2);

        var active = await repo.GetAllAsync(global::Data.Interfaz.IDataImplement.Business.ActivityFilter.ActiveOnly);
        active.Should().HaveCount(1);
        active.First().Id.Should().Be(1);
    }

    [Fact]
    public async Task GetBasicsByIdsAsync_ProjectsValues()
    {
        await using var ctx = Ctx();
        var repo = new EstablishmentsRepository(ctx);
        var plaza = new Plaza { Id = 1, Name = "P", Description = "", Active = true, Location = "L" };
        ctx.plaza.Add(plaza);
        ctx.Establishment.AddRange(
            new Establishment { Id = 10, Name = "A", Description = "", Active = true, PlazaId = 1, Plaza = plaza, RentValueBase = 100, UvtQty = 2 },
            new Establishment { Id = 11, Name = "B", Description = "", Active = false, PlazaId = 1, Plaza = plaza, RentValueBase = 200, UvtQty = 3 }
        );
        await ctx.SaveChangesAsync();

        var basics = await repo.GetBasicsByIdsAsync(new[] { 10, 11 });
        basics.Should().ContainSingle(x => x.Id == 10 && x.RentValueBase == 100 && x.UvtQty == 2);
    }

    [Fact]
    public async Task GetInactiveIdsAsync_ReturnsOnlyInactive()
    {
        await using var ctx = Ctx();
        var repo = new EstablishmentsRepository(ctx);
        var plaza = new Plaza { Id = 1, Name = "P", Description = "", Active = true, Location = "L" };
        ctx.plaza.Add(plaza);
        ctx.Establishment.AddRange(
            new Establishment { Id = 20, Name = "A", Description = "", Active = true, PlazaId = 1, Plaza = plaza },
            new Establishment { Id = 21, Name = "B", Description = "", Active = false, PlazaId = 1, Plaza = plaza }
        );
        await ctx.SaveChangesAsync();

        var inactive = await repo.GetInactiveIdsAsync(new[] { 20, 21 });
        inactive.Should().ContainSingle(i => i == 21);
    }

    // Nota: SetActiveByIdsAsync usa ExecuteUpdateAsync, no soportado por InMemory → mejor cubrir en integración.
}
