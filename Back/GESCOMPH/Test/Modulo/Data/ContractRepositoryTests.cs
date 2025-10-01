using Data.Services.Business;
using Entity.Domain.Models.Implements.Business;
using Entity.Domain.Models.Implements.Persons;
using Entity.Domain.Models.Implements.SecurityAuthentication;
using Entity.Infrastructure.Context;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace Tests.Data.Business;

public class ContractRepositoryTests
{
    private static ApplicationDbContext Ctx()
    {
        var opt = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new ApplicationDbContext(opt);
    }

    private static Person NewPerson(int id)
        => new Person { Id = id, FirstName = $"P{id}", LastName = "L", CityId = 1 };

    [Fact]
    public async Task GetCards_ProjectsFields()
    {
        await using var ctx = Ctx();
        var repo = new ContractRepository(ctx);

        var p = NewPerson(1);
        var u = new User { Id = 1, Email = "p@mail", Password = "x", PersonId = 1, Person = p };
        p.User = u;
        var c = new Contract { Id = 1, Person = p, PersonId = 1, StartDate = DateTime.Today, EndDate = DateTime.Today, TotalBaseRentAgreed = 10, TotalUvtQtyAgreed = 2, Active = true };

        ctx.Persons.Add(p);
        ctx.Users.Add(u);
        ctx.contracts.Add(c);
        await ctx.SaveChangesAsync();

        var all = await repo.GetCardsAllAsync();
        all.Should().ContainSingle(x => x.Id == 1 && x.PersonEmail == "p@mail");

        var byPerson = await repo.GetCardsByPersonAsync(1);
        byPerson.Should().ContainSingle(x => x.PersonId == 1);
    }

    [Fact(Skip="EFCore.InMemory no soporta ExecuteUpdate/ExecuteUpdateAsync")]
    public async Task DeactivateExpiredAsync_DisablesActiveEndedContracts()
    {
        await using var ctx = Ctx();
        var repo = new ContractRepository(ctx);
        var now = DateTime.UtcNow;

        ctx.Persons.Add(NewPerson(1));
        ctx.contracts.AddRange(
            new Contract { Id = 1, PersonId = 1, StartDate = now.AddMonths(-2), EndDate = now.AddDays(-1), Active = true },
            new Contract { Id = 2, PersonId = 1, StartDate = now.AddMonths(-2), EndDate = now.AddDays(10), Active = true },
            new Contract { Id = 3, PersonId = 1, StartDate = now.AddMonths(-3), EndDate = now.AddDays(-2), Active = false }
        );
        await ctx.SaveChangesAsync();

        var ids = await repo.DeactivateExpiredAsync(now);
        ids.Should().ContainSingle(i => i == 1);

        (await ctx.contracts.FindAsync(1))!.Active.Should().BeFalse();
        (await ctx.contracts.FindAsync(2))!.Active.Should().BeTrue();
    }

    [Fact(Skip="EFCore.InMemory no soporta ExecuteUpdate/ExecuteUpdateAsync")]
    public async Task ReleaseEstablishments_ActivatesOnlyFreeOnes()
    {
        await using var ctx = Ctx();
        var repo = new ContractRepository(ctx);
        var now = DateTime.UtcNow;

        ctx.Persons.Add(NewPerson(1));
        ctx.Establishment.AddRange(
            new Establishment { Id = 10, Name = "E1", Description = "", Active = false },
            new Establishment { Id = 11, Name = "E2", Description = "", Active = false }
        );

        // Contract 1 expired and inactive on est 10
        ctx.contracts.Add(new Contract { Id = 1, PersonId = 1, StartDate = now.AddMonths(-3), EndDate = now.AddDays(-1), Active = false });
        ctx.premisesLeaseds.Add(new PremisesLeased { Id = 1, ContractId = 1, EstablishmentId = 10 });

        // Contract 2 active (should keep est 11 occupied)
        ctx.contracts.Add(new Contract { Id = 2, PersonId = 1, StartDate = now.AddMonths(-1), EndDate = now.AddMonths(1), Active = true });
        ctx.premisesLeaseds.Add(new PremisesLeased { Id = 2, ContractId = 2, EstablishmentId = 11 });

        await ctx.SaveChangesAsync();

        var changed = await repo.ReleaseEstablishmentsForExpiredAsync(now);
        changed.Should().Be(1);

        (await ctx.Establishment.FindAsync(10))!.Active.Should().BeTrue();
        (await ctx.Establishment.FindAsync(11))!.Active.Should().BeFalse();
    }
}
