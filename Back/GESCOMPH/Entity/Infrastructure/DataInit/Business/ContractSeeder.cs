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
                // Contrato activo todo 2025 (Establecimientos: 1 y 5)
                new Contract
                {
                    Id = 1,
                    PersonId = 1,                      
                    StartDate = new DateTime(2025, 01, 01, 0, 0, 0, DateTimeKind.Utc),
                    EndDate = new DateTime(2025, 12, 31, 23, 59, 59, DateTimeKind.Utc),
                    TotalUvtQtyAgreed = 38m,            // 30 + 8 (Est. 1 y 5)
                    TotalBaseRentAgreed = 5_700_000m,   // 4.5M + 1.2M
                    Active = true,
                    IsDeleted = false,
                    CreatedAt = seedAt
                },
                // Otro contrato activo (mitad de año a mitad de 2026) (Establecimientos: 2 y 6)
                new Contract
                {
                    Id = 2,
                    PersonId = 1,                       
                    StartDate = new DateTime(2025, 07, 01, 0, 0, 0, DateTimeKind.Utc),
                    EndDate = new DateTime(2026, 06, 30, 23, 59, 59, DateTimeKind.Utc),
                    TotalUvtQtyAgreed = 48m,            // 22 + 26
                    TotalBaseRentAgreed = 7_100_000m,   // 3.2M + 3.9M
                    Active = true,
                    IsDeleted = false,
                    CreatedAt = seedAt
                },
                // Contrato expirado (no debería generar obligaciones en meses de 2025) (Establecimiento: 3)
                new Contract
                {
                    Id = 3,
                    PersonId = 1,                       
                    StartDate = new DateTime(2023, 01, 01, 0, 0, 0, DateTimeKind.Utc),
                    EndDate = new DateTime(2024, 06, 30, 23, 59, 59, DateTimeKind.Utc),
                    TotalUvtQtyAgreed = 45m,            
                    TotalBaseRentAgreed = 6_800_000m,
                    Active = false,
                    IsDeleted = false,
                    CreatedAt = seedAt
                }
            );
        }
    }
}
