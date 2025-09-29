using Entity.Domain.Models.Implements.Business;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Entity.Infrastructure.DataInit.Business
{
    public class EstablishmentSeeder : IEntityTypeConfiguration<Establishment>
    {
        public void Configure(EntityTypeBuilder<Establishment> builder)
        {
            var seedDate = new DateTime(2025, 01, 01, 0, 0, 0, DateTimeKind.Utc);

            builder.HasData(
                new Establishment
                {
                    Id = 1,
                    Name = "Centro Comercial Primavera",
                    Description = "Local comercial en centro urbano con alto flujo peatonal.",
                    AreaM2 = 120,
                    RentValueBase = 4_500_000m,
                    Address = "Cra. 15 # 93-60, Bogotá",
                    UvtQty = 30m,
                    PlazaId = 1, // Asumiendo que la Plaza con Id 1 ya existe
                    Active = true,
                    IsDeleted = false,
                    CreatedAt = seedDate
                },
                new Establishment
                {
                    Id = 2,
                    Name = "Oficina Torre Norte Piso 7",
                    Description = "Oficina empresarial con vista y sala de reuniones.",
                    AreaM2 = 85,
                    RentValueBase = 3_200_000m,
                    Address = "Av. El Dorado # 69-76, Bogotá",
                    UvtQty = 22m,
                    PlazaId = 2, // Asumiendo que la Plaza con Id 2 ya existe
                    Active = true,
                    IsDeleted = false,
                    CreatedAt = seedDate
                },
                new Establishment
                {
                    Id = 3,
                    Name = "Bodega Industrial Sur",
                    Description = "Bodega con muelle de carga y altura de 8m.",
                    AreaM2 = 750,
                    RentValueBase = 6_800_000m,
                    Address = "Cl. 57 Sur # 30-15, Bogotá",
                    UvtQty = 45m,
                    PlazaId = 1, // Asumiendo que la Plaza con Id 1 ya existe
                    Active = true,
                    IsDeleted = false,
                    CreatedAt = seedDate
                },
                new Establishment
                {
                    Id = 4,
                    Name = "Local Gastronómico Zona G",
                    Description = "Espacio para restaurante con extracción y bodega.",
                    AreaM2 = 95,
                    RentValueBase = 5_200_000m,
                    Address = "Cra. 5 # 69A-19, Bogotá",
                    UvtQty = 35m,
                    PlazaId = 1,
                    Active = true,
                    IsDeleted = false,
                    CreatedAt = seedDate
                },
                new Establishment
                {
                    Id = 5,
                    Name = "Isla Comercial Pasillo Central",
                    Description = "Isla en pasillo principal, ideal retail liviano.",
                    AreaM2 = 12,
                    RentValueBase = 1_200_000m,
                    Address = "Centro Comercial Primavera, pasillo central",
                    UvtQty = 8m,
                    PlazaId = 1,
                    Active = true,
                    IsDeleted = false,
                    CreatedAt = seedDate
                },
                new Establishment
                {
                    Id = 6,
                    Name = "Oficina Torre Norte Piso 12",
                    Description = "Oficina abierta con 2 parqueaderos y cableado estructurado.",
                    AreaM2 = 110,
                    RentValueBase = 3_900_000m,
                    Address = "Av. El Dorado # 69-76, Bogotá",
                    UvtQty = 26m,
                    PlazaId = 2,
                    Active = true,
                    IsDeleted = false,
                    CreatedAt = seedDate
                }
            );
        }
    }
}
