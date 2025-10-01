using Data.Services.SecurityAuthentication;
using Entity.Domain.Models.Implements.Location;
using Entity.Domain.Models.Implements.SecurityAuthentication;
using Entity.Infrastructure.Context;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace Tests.Data.SecurityAuthentication;

public class UserRepositoryTests
{
    private static ApplicationDbContext Ctx()
    {
        var opt = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new ApplicationDbContext(opt);
    }

    private static User NewUser(int id, string email) => new() { Id = id, Email = email, Password = "hash", PersonId = id };

    [Fact]
    public async Task ExistsByEmail_Variants()
    {
        await using var ctx = Ctx();
        var repo = new UserRepository(ctx);
        // Seed minimal graph for Person/City
        ctx.Set<Department>().Add(new Department { Id = 1, Name = "Dept" });
        ctx.Set<City>().Add(new City { Id = 1, Name = "Neiva", DepartmentId = 1 });
        ctx.Set<Entity.Domain.Models.Implements.Persons.Person>().Add(new Entity.Domain.Models.Implements.Persons.Person { Id = 1, FirstName = "A", LastName = "B", CityId = 1 });
        ctx.Set<Entity.Domain.Models.Implements.Persons.Person>().Add(new Entity.Domain.Models.Implements.Persons.Person { Id = 2, FirstName = "C", LastName = "D", CityId = 1 });
        ctx.Users.AddRange(NewUser(1, "a@mail"), NewUser(2, "b@mail"));
        await ctx.SaveChangesAsync();

        (await repo.ExistsByEmailAsync("A@MAIL")).Should().BeTrue();
        (await repo.ExistsByEmailExcludingIdAsync(1, "a@mail")).Should().BeFalse();
        (await repo.ExistsByEmailExcludingIdAsync(2, "a@mail")).Should().BeTrue();
        (await repo.GetIdByEmailAsync("b@mail")).Should().Be(2);
    }

    [Fact]
    public async Task GetByEmail_Variants()
    {
        await using var ctx = Ctx();
        var repo = new UserRepository(ctx);
        ctx.Set<Department>().Add(new Department { Id = 1, Name = "Dept" });
        ctx.Set<City>().Add(new City { Id = 1, Name = "Neiva", DepartmentId = 1 });
        ctx.Set<Entity.Domain.Models.Implements.Persons.Person>().Add(new Entity.Domain.Models.Implements.Persons.Person { Id = 1, FirstName = "A", LastName = "B", CityId = 1 });
        ctx.Set<Entity.Domain.Models.Implements.Persons.Person>().Add(new Entity.Domain.Models.Implements.Persons.Person { Id = 2, FirstName = "C", LastName = "D", CityId = 1 });
        ctx.Users.AddRange(NewUser(1, "a@mail"), NewUser(2, "b@mail"));
        await ctx.SaveChangesAsync();

        var full = await repo.GetByEmailAsync("a@mail");
        full.Should().NotBeNull();

        var projection = await repo.GetByEmailProjectionAsync("a@mail");
        projection!.Id.Should().Be(1);

        var auth = await repo.GetAuthUserByEmailAsync("a@mail");
        auth!.Password.Should().Be("hash");
    }

    [Fact]
    public async Task GetByPersonId_ReturnsUser()
    {
        await using var ctx = Ctx();
        var repo = new UserRepository(ctx);
        ctx.Set<Department>().Add(new Department { Id = 1, Name = "Dept" });
        ctx.Set<City>().Add(new City { Id = 1, Name = "Neiva", DepartmentId = 1 });
        ctx.Set<Entity.Domain.Models.Implements.Persons.Person>().Add(new Entity.Domain.Models.Implements.Persons.Person { Id = 1, FirstName = "A", LastName = "B", CityId = 1 });
        ctx.Users.Add(NewUser(1, "a@mail"));
        await ctx.SaveChangesAsync();

        var user = await repo.GetByPersonIdAsync(1);
        user.Should().NotBeNull();
    }

    [Fact]
    public async Task GetByIdWithDetails_ReturnsUser()
    {
        await using var ctx = Ctx();
        var repo = new UserRepository(ctx);
        ctx.Set<Department>().Add(new Department { Id = 1, Name = "Dept" });
        ctx.Set<City>().Add(new City { Id = 1, Name = "Neiva", DepartmentId = 1 });
        ctx.Set<Entity.Domain.Models.Implements.Persons.Person>().Add(new Entity.Domain.Models.Implements.Persons.Person { Id = 1, FirstName = "A", LastName = "B", CityId = 1 });
        ctx.Users.Add(NewUser(1, "a@mail"));
        await ctx.SaveChangesAsync();

        var user = await repo.GetByIdWithDetailsAsync(1);
        user.Should().NotBeNull();
    }
}
