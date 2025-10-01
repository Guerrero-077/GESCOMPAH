using Data.Services.SecurityAuthentication;
using Entity.Domain.Models.Implements.SecurityAuthentication;
using Entity.Infrastructure.Context;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace Test.Modulo.Data;

public class RefreshTokenRepositoryTests
{
    private static ApplicationDbContext Ctx()
    {
        var opt = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new ApplicationDbContext(opt);
    }

    [Fact]
    public async Task Add_GetByHash_Revoke_AndGetValidByUser()
    {
        await using var ctx = Ctx();
        var repo = new RefreshTokenRepository(ctx);

        var t1 = new RefreshToken { UserId = 1, TokenHash = "h1", ExpiresAt = DateTime.UtcNow.AddHours(1) };
        var t2 = new RefreshToken { UserId = 1, TokenHash = "h2", ExpiresAt = DateTime.UtcNow.AddHours(-1) }; // expirado
        await repo.AddAsync(t1);
        await repo.AddAsync(t2);

        var fetched = await repo.GetByHashAsync("h1");
        fetched.Should().NotBeNull();

        // válidos: solo h1 (no revocado y no expirado)
        var valid = await repo.GetValidTokensByUserAsync(1);
        valid.Should().ContainSingle(v => v.TokenHash == "h1");

        // Revocar h1 y marcar replacedBy
        await repo.RevokeAsync(new RefreshToken { Id = fetched!.Id }, replacedByTokenHash: "h3");

        var validAfter = await repo.GetValidTokensByUserAsync(1);
        validAfter.Should().BeEmpty();
    }
}

