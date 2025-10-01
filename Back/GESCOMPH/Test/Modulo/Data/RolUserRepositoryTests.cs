using Data.Services.SecurityAuthentication;
using Entity.Domain.Models.Implements.SecurityAuthentication;
using Entity.Infrastructure.Context;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace Test.Modulo.Data;

public class RolUserRepositoryTests
{
    private static ApplicationDbContext Ctx()
    {
        var opt = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new ApplicationDbContext(opt);
    }

    [Fact]
    public async Task AsignateRolDefault_CreatesRolUser()
    {
        await using var ctx = Ctx();
        var repo = new RolUserRepository(ctx);
        // Seed rol with Id=2 (default)
        ctx.Roles.Add(new Rol { Id = 2, Name = "User", Description = "U" });
        var user = new User { Id = 1, Email = "u@mail", Password = "x", PersonId = 1 };
        ctx.Users.Add(user);
        await ctx.SaveChangesAsync();

        var ru = await repo.AsignateRolDefault(user);
        ru.UserId.Should().Be(1);
        ru.RolId.Should().Be(2);
        (await ctx.RolUsers.CountAsync()).Should().Be(1);
    }

    [Fact]
    public async Task GetRolesByUserIdAsync_ReturnsRoles()
    {
        await using var ctx = Ctx();
        var repo = new RolUserRepository(ctx);
        var admin = new Rol { Id = 1, Name = "Admin", Description = "A" };
        var user = new Rol { Id = 2, Name = "User", Description = "U" };
        ctx.Roles.AddRange(admin, user);
        ctx.Users.Add(new User { Id = 1, Email = "a@mail", Password = "x", PersonId = 1 });
        ctx.RolUsers.AddRange(
            new RolUser { Id = 1, UserId = 1, RolId = 1, Active = true },
            new RolUser { Id = 2, UserId = 1, RolId = 2, Active = true }
        );
        await ctx.SaveChangesAsync();

        var roles = await repo.GetRolesByUserIdAsync(1);
        roles.Should().HaveCount(2);
        roles.Select(r => r.Name).Should().BeEquivalentTo(new[] { "Admin", "User" });
    }

    [Fact]
    public async Task GetRoleNamesByUserIdAsync_OnlyActiveDistinct()
    {
        await using var ctx = Ctx();
        var repo = new RolUserRepository(ctx);
        var admin = new Rol { Id = 1, Name = "Admin", Description = "A" };
        var dup = new Rol { Id = 3, Name = "Admin", Description = "A" };
        var disabled = new Rol { Id = 2, Name = "Old", Description = "O" };
        ctx.Roles.AddRange(admin, dup, disabled);
        ctx.Users.Add(new User { Id = 1, Email = "a@mail", Password = "x", PersonId = 1 });
        ctx.RolUsers.AddRange(
            new RolUser { Id = 1, UserId = 1, RolId = 1, Active = true },
            new RolUser { Id = 2, UserId = 1, RolId = 2, Active = false },
            new RolUser { Id = 3, UserId = 1, RolId = 3, Active = true }
        );
        await ctx.SaveChangesAsync();

        var names = (await repo.GetRoleNamesByUserIdAsync(1)).ToList();
        names.Should().HaveCount(1);
        names[0].Should().Be("Admin");
    }

    [Fact]
    public async Task ReplaceUserRolesAsync_ReplacesSet()
    {
        await using var ctx = Ctx();
        var repo = new RolUserRepository(ctx);
        ctx.Roles.AddRange(
            new Rol { Id = 1, Name = "Admin", Description = "A" },
            new Rol { Id = 2, Name = "User", Description = "U" },
            new Rol { Id = 3, Name = "Manager", Description = "M" }
        );
        ctx.Users.Add(new User { Id = 1, Email = "a@mail", Password = "x", PersonId = 1 });
        ctx.RolUsers.AddRange(
            new RolUser { Id = 1, UserId = 1, RolId = 1, Active = true },
            new RolUser { Id = 2, UserId = 1, RolId = 2, Active = true }
        );
        await ctx.SaveChangesAsync();

        await repo.ReplaceUserRolesAsync(1, new[] { 2, 3 });

        var roles = await repo.GetRoleNamesByUserIdAsync(1);
        roles.Should().BeEquivalentTo(new[] { "User", "Manager" });
    }
}
