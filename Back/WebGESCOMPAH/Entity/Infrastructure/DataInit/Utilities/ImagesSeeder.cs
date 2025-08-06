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
                new Images
                {
                    Id = 1,
                    FileName = "primavera_1.jpg",
                    FilePath = "https://res.cloudinary.com/dmbndpjlh/image/upload/v1754490905/establishments/6/Gemini_Generated_Image_1x3mol1x3mol1x3m_%281%29_55ab90047aae467988180701fe1bb6f1.png",
                    PublicId = "primavera_1", // <-- agregado
                    EstablishmentId = 1,
                    Active = true,
                    IsDeleted = false,
                    CreatedAt = seedDate
                },
                new Images
                {
                    Id = 2,
                    FileName = "primavera_2.jpg",
                    FilePath = "https://res.cloudinary.com/dmbndpjlh/image/upload/v1754490905/establishments/6/Gemini_Generated_Image_1x3mol1x3mol1x3m_%281%29_55ab90047aae467988180701fe1bb6f1.png",
                    PublicId = "primavera_2",
                    EstablishmentId = 1,
                    Active = true,
                    IsDeleted = false,
                    CreatedAt = seedDate
                },
                new Images
                {
                    Id = 3,
                    FileName = "torre_1.jpg",
                    FilePath = "https://res.cloudinary.com/dmbndpjlh/image/upload/v1754490905/establishments/6/Gemini_Generated_Image_1x3mol1x3mol1x3m_%281%29_55ab90047aae467988180701fe1bb6f1.png",
                    PublicId = "torre_1",
                    EstablishmentId = 2,
                    Active = true,
                    IsDeleted = false,
                    CreatedAt = seedDate
                },
                new Images
                {
                    Id = 4,
                    FileName = "torre_2.jpg",
                    FilePath = "https://res.cloudinary.com/dmbndpjlh/image/upload/v1754490905/establishments/6/Gemini_Generated_Image_1x3mol1x3mol1x3m_%281%29_55ab90047aae467988180701fe1bb6f1.png",
                    PublicId = "torre_2",
                    EstablishmentId = 2,
                    Active = true,
                    IsDeleted = false,
                    CreatedAt = seedDate
                },
                new Images
                {
                    Id = 5,
                    FileName = "bodega_1.jpg",
                    FilePath = "https://res.cloudinary.com/dmbndpjlh/image/upload/v1754490905/establishments/6/Gemini_Generated_Image_1x3mol1x3mol1x3m_%281%29_55ab90047aae467988180701fe1bb6f1.png",
                    PublicId = "bodega_1",
                    EstablishmentId = 3,
                    Active = true,
                    IsDeleted = false,
                    CreatedAt = seedDate
                },
                new Images
                {
                    Id = 6,
                    FileName = "bodega_2.jpg",
                    FilePath = "https://res.cloudinary.com/dmbndpjlh/image/upload/v1754490905/establishments/6/Gemini_Generated_Image_1x3mol1x3mol1x3m_%281%29_55ab90047aae467988180701fe1bb6f1.png",
                    PublicId = "bodega_2",
                    EstablishmentId = 3,
                    Active = true,
                    IsDeleted = false,
                    CreatedAt = seedDate
                }
            );
        }

    }
}
