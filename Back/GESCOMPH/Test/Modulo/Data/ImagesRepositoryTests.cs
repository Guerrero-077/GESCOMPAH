using Data.Services.Utilities;
using Entity.Domain.Models.Implements.Utilities;
using Entity.Infrastructure.Context;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace Tests.Data.Utilities;

public class ImagesRepositoryTests
{
    private static ApplicationDbContext Ctx()
    {
        var opt = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new ApplicationDbContext(opt);
    }

    [Fact]
    public async Task GetAll_ReturnsOnlyActiveNotDeleted_Ordered()
    {
        await using var ctx = Ctx();
        var repo = new ImagesRepository(ctx);

        ctx.Images.AddRange(
            new Images { FileName = "a.jpg", FilePath = "/a", PublicId = "p1", EstablishmentId = 1, Active = true, IsDeleted = false, CreatedAt = DateTime.UtcNow.AddDays(-1) },
            new Images { FileName = "b.jpg", FilePath = "/b", PublicId = "p2", EstablishmentId = 1, Active = false, IsDeleted = false },
            new Images { FileName = "c.jpg", FilePath = "/c", PublicId = "p3", EstablishmentId = 1, Active = true, IsDeleted = true },
            new Images { FileName = "d.jpg", FilePath = "/d", PublicId = "p4", EstablishmentId = 1, Active = true, IsDeleted = false, CreatedAt = DateTime.UtcNow }
        );
        await ctx.SaveChangesAsync();

        var all = (await repo.GetAllAsync()).ToList();
        all.Should().HaveCount(2);
        all.First().PublicId.Should().Be("p4"); // más reciente primero
        all.Last().PublicId.Should().Be("p1");
    }

    [Fact]
    public async Task GetByEstablishmentId_FiltersAndOrders()
    {
        await using var ctx = Ctx();
        var repo = new ImagesRepository(ctx);

        ctx.Images.AddRange(
            new Images { FileName = "a.jpg", FilePath = "/a", PublicId = "p1", EstablishmentId = 9, Active = true },
            new Images { FileName = "b.jpg", FilePath = "/b", PublicId = "p2", EstablishmentId = 9, Active = true },
            new Images { FileName = "c.jpg", FilePath = "/c", PublicId = "p3", EstablishmentId = 8, Active = true }
        );
        await ctx.SaveChangesAsync();

        var list = await repo.GetByEstablishmentIdAsync(9);
        list.Should().HaveCount(2);
        list.First().PublicId.Should().Be("p2"); // ordenado desc por Id
    }

    [Fact]
    public async Task AddRange_AndDeleteByPublicId_Works()
    {
        await using var ctx = Ctx();
        var repo = new ImagesRepository(ctx);

        await repo.AddRangeAsync(new[]
        {
            new Images { FileName = "a.jpg", FilePath = "/a", PublicId = "pa", EstablishmentId = 1 },
            new Images { FileName = "b.jpg", FilePath = "/b", PublicId = "pb", EstablishmentId = 1 }
        });

        (await repo.DeleteByPublicIdAsync("pa")).Should().BeTrue();
        (await repo.DeleteByPublicIdAsync("nope")).Should().BeFalse();
    }

    [Fact]
    public async Task DeleteLogicalByPublicId_Works()
    {
        await using var ctx = Ctx();
        var repo = new ImagesRepository(ctx);

        await repo.AddRangeAsync(new[] { new Images { FileName = "a.jpg", FilePath = "/a", PublicId = "pa", EstablishmentId = 1 } });
        (await repo.DeleteLogicalByPublicIdAsync("pa")).Should().BeTrue();
        (await repo.DeleteLogicalByPublicIdAsync("")).Should().BeFalse();
    }
}

