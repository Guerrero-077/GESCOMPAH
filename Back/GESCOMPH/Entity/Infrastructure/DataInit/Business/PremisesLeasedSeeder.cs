using Entity.Domain.Models.Implements.Business;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Entity.Infrastructure.DataInit.Business
{
    public sealed class PremisesLeasedSeeder : IEntityTypeConfiguration<PremisesLeased>
    {
        public void Configure(EntityTypeBuilder<PremisesLeased> builder)
        {
            var seedAt = new DateTime(2025, 01, 01, 0, 0, 0, DateTimeKind.Utc);

            // 🔒 Evita duplicados (si no lo haces en OnModelCreating global)
            builder.HasIndex(x => new { x.ContractId, x.EstablishmentId }).IsUnique();

            builder.HasData(
                // Contrato 1001 usa el establecimiento 1
                new PremisesLeased
                {
                    Id = 1,
                    ContractId = 1,        // ← existe por ContractSeeder
                    EstablishmentId = 1,   // ← existe en EstablishmentSeeder
                    Active = true,
                    IsDeleted = false,
                    CreatedAt = seedAt
                },
                // Contrato 1002 usa el establecimiento 2
                new PremisesLeased
                {
                    Id = 2,
                    ContractId = 2,
                    EstablishmentId = 2,
                    Active = true,
                    IsDeleted = false,
                    CreatedAt = seedAt
                },
                // Contrato 3 (histórico) vinculado al establecimiento 3
                new PremisesLeased
                {
                    Id = 3,
                    ContractId = 3,
                    EstablishmentId = 3,
                    Active = true,
                    IsDeleted = false,
                    CreatedAt = seedAt
                },
                // Contrato 1 también usa el establecimiento 5 (isla comercial)
                new PremisesLeased
                {
                    Id = 4,
                    ContractId = 1,
                    EstablishmentId = 5,
                    Active = true,
                    IsDeleted = false,
                    CreatedAt = seedAt
                },
                // Contrato 2 también usa el establecimiento 6 (oficina piso 12)
                new PremisesLeased
                {
                    Id = 5,
                    ContractId = 2,
                    EstablishmentId = 6,
                    Active = true,
                    IsDeleted = false,
                    CreatedAt = seedAt
                }
                // (opcional) Si quieres que 1 tenga dos locales:
                // ,new PremisesLeased
                // {
                //     Id = 4,
                //     ContractId = 1,
                //     EstablishmentId = 2,
                //     Active = true,
                //     IsDeleted = false,
                //     CreatedAt = seedAt
                // }
            );
        }
    }
}
