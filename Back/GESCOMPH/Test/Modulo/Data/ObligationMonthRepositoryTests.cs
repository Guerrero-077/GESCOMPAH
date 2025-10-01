using Data.Services.Business;
using Entity.Domain.Models.Implements.Business;
using Entity.Infrastructure.Context;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace Test.Modulo.Data;

public class ObligationMonthRepositoryTests
{
    private static ApplicationDbContext Ctx()
    {
        var opt = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new ApplicationDbContext(opt);
    }

    [Fact]
    public async Task GetByContractYearMonth_ReturnsExpected()
    {
        await using var ctx = Ctx();
        var repo = new ObligationMonthRepository(ctx);
        ctx.obligationMonths.AddRange(
            new ObligationMonth { Id = 1, ContractId = 5, Year = 2024, Month = 1, DueDate = DateTime.Today, UvtQtyApplied = 0, UvtValueApplied = 0, VatRateApplied = 0, BaseAmount = 0, VatAmount = 0, TotalAmount = 0, Status = "P" },
            new ObligationMonth { Id = 2, ContractId = 5, Year = 2024, Month = 2, DueDate = DateTime.Today, UvtQtyApplied = 0, UvtValueApplied = 0, VatRateApplied = 0, BaseAmount = 0, VatAmount = 0, TotalAmount = 0, Status = "P" }
        );
        await ctx.SaveChangesAsync();

        var hit = await repo.GetByContractYearMonthAsync(5, 2024, 2);
        hit.Should().NotBeNull();

        var miss = await repo.GetByContractYearMonthAsync(5, 2023, 12);
        miss.Should().BeNull();
    }

    [Fact]
    public async Task GetByContractQueryable_OrdersDesc_AndFiltersDeleted()
    {
        await using var ctx = Ctx();
        var repo = new ObligationMonthRepository(ctx);
        ctx.obligationMonths.AddRange(
            new ObligationMonth { Id = 1, ContractId = 7, Year = 2023, Month = 12, DueDate = DateTime.Today, UvtQtyApplied = 0, UvtValueApplied = 0, VatRateApplied = 0, BaseAmount = 0, VatAmount = 0, TotalAmount = 0, Status = "P" },
            new ObligationMonth { Id = 2, ContractId = 7, Year = 2024, Month = 1, DueDate = DateTime.Today, UvtQtyApplied = 0, UvtValueApplied = 0, VatRateApplied = 0, BaseAmount = 0, VatAmount = 0, TotalAmount = 0, Status = "P" , IsDeleted = true},
            new ObligationMonth { Id = 3, ContractId = 7, Year = 2024, Month = 2, DueDate = DateTime.Today, UvtQtyApplied = 0, UvtValueApplied = 0, VatRateApplied = 0, BaseAmount = 0, VatAmount = 0, TotalAmount = 0, Status = "P" }
        );
        await ctx.SaveChangesAsync();

        var list = repo.GetByContractQueryable(7).ToList();
        list.Should().HaveCount(2);
        list.First().Month.Should().Be(2);
    }
}

