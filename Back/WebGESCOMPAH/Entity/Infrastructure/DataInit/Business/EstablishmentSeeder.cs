using Entity.Domain.Models.Implements.Business;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Entity.Infrastructure.DataInit.Business
{
    public class EstablishmentSeeder : IEntityTypeConfiguration<Establishment>
    {
        public void Configure(EntityTypeBuilder<Establishment> builder)
        {
            var seedDate = new DateTime(2025, 01, 01);

            builder.HasData(
                new Establishment
                {
                    Id = 1,
                    Name = "Centro Comercial Primavera",
                    Description = "Establecimiento amplio con excelente ubicación.",
                    AreaM2 = 500,
                    RentValueBase = 2500,
                    Address = "Cr 1 ",
                    PlazaId = 1, // Asumiendo que la Plaza con Id 1 ya existe
                    Active = true,
                    IsDeleted = false,
                    CreatedAt = seedDate
                },
                new Establishment
                {
                    Id = 2,
                    Name = "Oficina Torre Norte",
                    Description = "Oficina moderna en zona empresarial.",
                    AreaM2 = 120,
                    RentValueBase = 1500,
                    Address = "Cr 1 ",
                    PlazaId = 2, // Asumiendo que la Plaza con Id 1 ya existe
                    Active = true,
                    IsDeleted = false,
                    CreatedAt = seedDate
                },
                new Establishment
                {
                    Id = 3,
                    Name = "Bodega Industrial Sur",
                    Description = "Espacio para almacenamiento de gran capacidad.",
                    AreaM2 = 1000,
                    RentValueBase = 3000,
                    Address = "Cr 1 ",
                    PlazaId = 1, // Asumiendo que la Plaza con Id 1 ya existe
                    Active = true,
                    IsDeleted = false,
                    CreatedAt = seedDate
                }
            );
        }
    }
}
