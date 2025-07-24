using Entity.Domain.Models.Implements.Utilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Entity.Infrastructure.DataInit.Utilities
{
    public class ImagesSeeder : IEntityTypeConfiguration<Images>
    {
        public void Configure(EntityTypeBuilder<Images> builder)
        {
            var seedDate = new DateTime(2025, 01, 01);

            builder.HasData(
                // Establecimiento 1
                new Images
                {
                    Id = 1,
                    FileName = "primavera_1.jpg",
                    FilePath = "https://cdn.app.com/establishments/primavera_1.jpg",
                    EstablishmentId = 1,
                    Active = true,
                    IsDeleted = false,
                    CreatedAt = seedDate
                },
                new Images
                {
                    Id = 2,
                    FileName = "primavera_2.jpg",
                    FilePath = "https://cdn.app.com/establishments/primavera_2.jpg",
                    EstablishmentId = 1,
                    Active = true,
                    IsDeleted = false,
                    CreatedAt = seedDate
                },

                // Establecimiento 2
                new Images
                {
                    Id = 3,
                    FileName = "torre_1.jpg",
                    FilePath = "https://cdn.app.com/establishments/torre_1.jpg",
                    EstablishmentId = 2,
                    Active = true,
                    IsDeleted = false,
                    CreatedAt = seedDate
                },
                new Images
                {
                    Id = 4,
                    FileName = "torre_2.jpg",
                    FilePath = "https://cdn.app.com/establishments/torre_2.jpg",
                    EstablishmentId = 2,
                    Active = true,
                    IsDeleted = false,
                    CreatedAt = seedDate
                },

                // Establecimiento 3
                new Images
                {
                    Id = 5,
                    FileName = "bodega_1.jpg",
                    FilePath = "https://cdn.app.com/establishments/bodega_1.jpg",
                    EstablishmentId = 3,
                    Active = true,
                    IsDeleted = false,
                    CreatedAt = seedDate
                },
                new Images
                {
                    Id = 6,
                    FileName = "bodega_2.jpg",
                    FilePath = "https://cdn.app.com/establishments/bodega_2.jpg",
                    EstablishmentId = 3,
                    Active = true,
                    IsDeleted = false,
                    CreatedAt = seedDate
                }
            );
        }
    }
}
