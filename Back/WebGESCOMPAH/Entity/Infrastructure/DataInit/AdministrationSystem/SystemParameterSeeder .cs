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

            // Evitar duplicados por nombre (Key) sin importar fechas
            builder.HasIndex(x => x.Key).IsUnique();

            builder.HasData(
                // UVT vigente (2025 en adelante)
                new SystemParameter
                {
                    Id = 2,
                    Key = "UVT",
                    Value = "51300",                   // Valor oficial 2025: confirmar con DIAN
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
