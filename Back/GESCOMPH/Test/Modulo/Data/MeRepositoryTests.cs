using Data.Services.SecurityAuthentication;
using Entity.Domain.Models.Implements.AdministrationSystem;
using Entity.Domain.Models.Implements.SecurityAuthentication;
using Entity.Infrastructure.Context;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace Test.Modulo.Data;

public class MeRepositoryTests
{
    private static ApplicationDbContext Ctx()
    {
        var opt = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new ApplicationDbContext(opt);
    }

    [Fact]
    public async Task GetUserWithPersonAsync_ReturnsUser()
    {
        await using var ctx = Ctx();
        var repo = new MeRepository(ctx);
        ctx.Users.Add(new User { Id = 1, Email = "u@mail", Password = "x", PersonId = 1, Person = new Entity.Domain.Models.Implements.Persons.Person { Id = 1, FirstName = "A", LastName = "B", CityId = 1 } });
        await ctx.SaveChangesAsync();
        var user = await repo.GetUserWithPersonAsync(1);
        user.Should().NotBeNull();
        user!.Person.Should().NotBeNull();
    }

    [Fact]
    public async Task GetFormsWithModulesByIdsAsync_ReturnsForms()
    {
        await using var ctx = Ctx();
        var repo = new MeRepository(ctx);
        ctx.Modules.Add(new Module { Id = 1, Name = "M", Description = "D", Icon = "i" });
        ctx.Forms.Add(new Form { Id = 2, Name = "F", Description = "D", Route = "/f" });
        ctx.FormModules.Add(new FormModule { Id = 3, FormId = 2, ModuleId = 1 });
        await ctx.SaveChangesAsync();

        var forms = (await repo.GetFormsWithModulesByIdsAsync(new List<int> { 2 })).ToList();
        forms.Should().ContainSingle(f => f.Id == 2 && f.FormModules.Any());
    }
}

