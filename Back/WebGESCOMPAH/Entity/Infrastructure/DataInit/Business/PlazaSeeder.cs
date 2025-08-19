using Entity.Domain.Models.Implements.Business;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Entity.Infrastructure.DataInit.Business
{
    public class PlazaSeeder : IEntityTypeConfiguration<Plaza>
    {
        public void Configure(EntityTypeBuilder<Plaza> builder)
        {
            var seedDate = new DateTime(2025, 01, 01, 0, 0, 0, DateTimeKind.Utc);

            builder.HasData(
                new Plaza
                {
                    Id = 1,
                    Name = "Plaza Central",
                    Description = "Espacio principal para eventos masivos",
                    Location = "Centro Ciudad",
                    Active = true,
                    IsDeleted = false,
                    CreatedAt = seedDate
                },
                new Plaza
                {
                    Id = 2,
                    Name = "Plaza Norte",
                    Description = "Zona adecuada para ferias temporales",
                    Location = "Zona Norte",
                    Active = true,
                    IsDeleted = false,
                    CreatedAt = seedDate
                }
            );
        }
    }
}
