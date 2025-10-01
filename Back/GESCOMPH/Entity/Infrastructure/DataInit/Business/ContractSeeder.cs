using Entity.Domain.Models.Implements.Business;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Entity.Infrastructure.DataInit.Business
{
    public sealed class ContractSeeder : IEntityTypeConfiguration<Contract>
    {
        public void Configure(EntityTypeBuilder<Contract> builder)
        {
            var seedAt = new DateTime(2025, 01, 01, 0, 0, 0, DateTimeKind.Utc);

            builder.HasData(
                // Contrato activo todo 2025
                new Contract
                {
                    Id = 1,
                    PersonId = 1,                       // ⚠️ Debe existir
                    StartDate = new DateTime(2025, 01, 01, 0, 0, 0, DateTimeKind.Utc),
                    EndDate = new DateTime(2025, 12, 31, 23, 59, 59, DateTimeKind.Utc),
                    TotalUvtQtyAgreed = 30m,            // 30 UVT
                    Active = true,
                    IsDeleted = false,
                    CreatedAt = seedAt
                },
                // Otro contrato activo (mitad de año a mitad de 2026)
                new Contract
                {
                    Id = 2,
                    PersonId = 1,                       // ⚠️ Debe existir
                    StartDate = new DateTime(2025, 07, 01, 0, 0, 0, DateTimeKind.Utc),
                    EndDate = new DateTime(2026, 06, 30, 23, 59, 59, DateTimeKind.Utc),
                    TotalUvtQtyAgreed = 45m,            // 45 UVT
                    Active = true,
                    IsDeleted = false,
                    CreatedAt = seedAt
                },
                // Contrato expirado (no debería generar obligaciones en meses de 2025)
                new Contract
                {
                    Id = 3,
                    PersonId = 1,                       // ⚠️ Debe existir
                    StartDate = new DateTime(2023, 01, 01, 0, 0, 0, DateTimeKind.Utc),
                    EndDate = new DateTime(2024, 06, 30, 23, 59, 59, DateTimeKind.Utc),
                    TotalUvtQtyAgreed = 20m,            // 20 UVT
                    Active = false,
                    IsDeleted = false,
                    CreatedAt = seedAt
                }
            );
        }
    }
}