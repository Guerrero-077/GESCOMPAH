using Data.Repositories.Implementations.SecurityAuthentication;
using Entity.Domain.Models.Implements.AdministrationSystem;
using Entity.Domain.Models.Implements.SecurityAuthentication;
using Entity.Infrastructure.Context;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace Tests.Data.AdministrationSystem;

public class RolFormPermissionRepositoryTests
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
        var repo = new RolFormPermissionRepository(ctx);
        ctx.Roles.Add(new Rol { Id = 1, Name = "R", Description = "D" });
        ctx.Forms.Add(new Form { Id = 2, Name = "F", Description = "D", Route = "/f" });
        ctx.Permissions.Add(new Permission { Id = 3, Name = "P", Description = "D" });
        await ctx.SaveChangesAsync();

        var rf = await repo.AddAsync(new RolFormPermission { RolId = 1, FormId = 2, PermissionId = 3 });
        rf.Id.Should().BeGreaterThan(0);

        var byId = await repo.GetByIdAsync(rf.Id);
        byId.Should().NotBeNull();

        await repo.UpdateAsync(new RolFormPermission { Id = rf.Id, RolId = 1, FormId = 2, PermissionId = 3 });
        (await repo.GetByIdAsync(rf.Id)).Should().NotBeNull();

        (await repo.DeleteLogicAsync(rf.Id)).Should().BeTrue();
        (await repo.GetByIdAsync(rf.Id)).Should().BeNull();
    }
}
