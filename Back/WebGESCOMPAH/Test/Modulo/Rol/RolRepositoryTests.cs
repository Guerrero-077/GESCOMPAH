using Entity.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using Data.Repository;

namespace Test.Modulo.Rol
{
    public class RolRepositoryTests
    {
        private static ApplicationDbContext BuildContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;
            return new ApplicationDbContext(options);
        }

        [Fact]
        public async Task AddAsync()
        {
            using var ctx = BuildContext(Guid.NewGuid().ToString());
            var repo = new DataGeneric<Entity.Domain.Models.Implements.SecurityAuthentication.Rol>(ctx);
            var rol = new Entity.Domain.Models.Implements.SecurityAuthentication.Rol { Name = "Admin", Description = "A" };

            var created = await repo.AddAsync(rol);

            var total = await ctx.Set<Entity.Domain.Models.Implements.SecurityAuthentication.Rol>().CountAsync();
            Assert.Equal(1, total);
            Assert.Equal("Admin", created.Name);
        }

        [Fact]
        public async Task GetAllAsync()
        {
            using var ctx = BuildContext(Guid.NewGuid().ToString());
            ctx.Set<Entity.Domain.Models.Implements.SecurityAuthentication.Rol>().AddRange(
                new Entity.Domain.Models.Implements.SecurityAuthentication.Rol { Name = "A", Description = "A", IsDeleted = false },
                new Entity.Domain.Models.Implements.SecurityAuthentication.Rol { Name = "B", Description = "B", IsDeleted = true },
                new Entity.Domain.Models.Implements.SecurityAuthentication.Rol { Name = "C", Description = "C", IsDeleted = false }
            );
            await ctx.SaveChangesAsync();
            var repo = new DataGeneric<Entity.Domain.Models.Implements.SecurityAuthentication.Rol>(ctx);

            var list = await repo.GetAllAsync();

            Assert.Equal(2, list.Count());
            Assert.DoesNotContain(list, x => x.Name == "B");
        }

        [Fact]
        public async Task GetByIdAsync()
        {
            using var ctx = BuildContext(Guid.NewGuid().ToString());
            var active = new Entity.Domain.Models.Implements.SecurityAuthentication.Rol { Name = "QA", Description = "X", IsDeleted = false };
            ctx.Set<Entity.Domain.Models.Implements.SecurityAuthentication.Rol>().Add(active);
            await ctx.SaveChangesAsync();
            var repo = new DataGeneric<Entity.Domain.Models.Implements.SecurityAuthentication.Rol>(ctx);

            var found = await repo.GetByIdAsync(active.Id);

            Assert.NotNull(found);
            Assert.Equal("QA", found!.Name);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnNull_WhenDeletedLogically()
        {
            using var ctx = BuildContext(Guid.NewGuid().ToString());
            var deleted = new Entity.Domain.Models.Implements.SecurityAuthentication.Rol { Name = "Old", Description = "Y", IsDeleted = true };
            ctx.Set<Entity.Domain.Models.Implements.SecurityAuthentication.Rol>().Add(deleted);
            await ctx.SaveChangesAsync();
            var repo = new DataGeneric<Entity.Domain.Models.Implements.SecurityAuthentication.Rol>(ctx);

            var found = await repo.GetByIdAsync(deleted.Id);

            Assert.Null(found);
        }

        [Fact]
        public async Task UpdateAsync()
        {
            using var ctx = BuildContext(Guid.NewGuid().ToString());
            var original = new Entity.Domain.Models.Implements.SecurityAuthentication.Rol { Name = "Producer", Description = "P" };
            ctx.Set<Entity.Domain.Models.Implements.SecurityAuthentication.Rol>().Add(original);
            await ctx.SaveChangesAsync();
            var repo = new DataGeneric<Entity.Domain.Models.Implements.SecurityAuthentication.Rol>(ctx);

            var toUpdate = new Entity.Domain.Models.Implements.SecurityAuthentication.Rol
            {
                Id = original.Id,
                Name = "Producer-EDIT",
                Description = "Edited"
            };

            var updated = await repo.UpdateAsync(toUpdate);

            Assert.Equal("Producer-EDIT", updated.Name);
            Assert.Equal("Edited", updated.Description);

            var reloaded = await ctx.Set<Entity.Domain.Models.Implements.SecurityAuthentication.Rol>().FindAsync(original.Id);
            Assert.Equal("Producer-EDIT", reloaded!.Name);
            Assert.Equal("Edited", reloaded.Description);
        }

        [Fact]
        public async Task DeleteLogicAsync()
        {
            using var ctx = BuildContext(Guid.NewGuid().ToString());
            var r = new Entity.Domain.Models.Implements.SecurityAuthentication.Rol { Name = "Temp", Description = "T" };
            ctx.Set<Entity.Domain.Models.Implements.SecurityAuthentication.Rol>().Add(r);
            await ctx.SaveChangesAsync();
            var repo = new DataGeneric<Entity.Domain.Models.Implements.SecurityAuthentication.Rol>(ctx);

            var ok = await repo.DeleteLogicAsync(r.Id);

            Assert.True(ok);

            var list = await repo.GetAllAsync();
            Assert.Empty(list);

            var raw = await ctx.Set<Entity.Domain.Models.Implements.SecurityAuthentication.Rol>()
                               .AsNoTracking()
                               .FirstAsync(x => x.Id == r.Id);
            Assert.True(raw.IsDeleted);
        }

        [Fact]
        public async Task DeleteAsync()
        {
            using var ctx = BuildContext(Guid.NewGuid().ToString());
            var r = new Entity.Domain.Models.Implements.SecurityAuthentication.Rol { Name = "X", Description = "Xd" };
            ctx.Set<Entity.Domain.Models.Implements.SecurityAuthentication.Rol>().Add(r);
            await ctx.SaveChangesAsync();
            var repo = new DataGeneric<Entity.Domain.Models.Implements.SecurityAuthentication.Rol>(ctx);

            var ok = await repo.DeleteAsync(r.Id);

            Assert.True(ok);
            Assert.Equal(0, await ctx.Set<Entity.Domain.Models.Implements.SecurityAuthentication.Rol>().CountAsync());
        }

        [Fact]
        public async Task DeleteAsync_ShouldReturnFalse_WhenEntityNotFound()
        {
            using var ctx = BuildContext(Guid.NewGuid().ToString());
            var repo = new DataGeneric<Entity.Domain.Models.Implements.SecurityAuthentication.Rol>(ctx);

            var ok = await repo.DeleteAsync(999);

            Assert.False(ok);
        }
    }
}
