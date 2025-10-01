using Entity.Domain.Models.Implements.AdministrationSystem;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Entity.Infrastructure.DataInit.AdministrationSystem
{
    public sealed class SystemParameterSeeder : IEntityTypeConfiguration<SystemParameter>
    {
        public void Configure(EntityTypeBuilder<SystemParameter> builder)
        {
            var seedAt = new DateTime(2025, 01, 01, 0, 0, 0, DateTimeKind.Utc);

            builder.HasData(
                // UVT 2024 (cerrado)
                new SystemParameter
                {
                    Id = 1,
                    Key = "UVT",
                    Value = "47000",                   // valor de ejemplo
                    EffectiveFrom = new DateTime(2024, 01, 01, 0, 0, 0, DateTimeKind.Utc),
                    EffectiveTo = new DateTime(2024, 12, 31, 23, 59, 59, DateTimeKind.Utc),
                    Active = true,
                    IsDeleted = false,
                    CreatedAt = seedAt
                },
                // UVT 2025 (abierto)
                new SystemParameter
                {
                    Id = 2,
                    Key = "UVT",
                    Value = "51000",                   // valor de ejemplo
                    EffectiveFrom = new DateTime(2025, 01, 01, 0, 0, 0, DateTimeKind.Utc),
                    EffectiveTo = null,
                    Active = true,
                    IsDeleted = false,
                    CreatedAt = seedAt
                },
                // IVA (19%) vigente
                new SystemParameter
                {
                    Id = 3,
                    Key = "IVA",
                    Value = "0.19",
                    EffectiveFrom = new DateTime(2020, 01, 01, 0, 0, 0, DateTimeKind.Utc),
                    EffectiveTo = null,
                    Active = true,
                    IsDeleted = false,
                    CreatedAt = seedAt
                }
            );
        }
    }
}