using Entity.Domain.Models.Implements.Location;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Entity.Infrastructure.DataInit.Location
{
    public class CitySeeder : IEntityTypeConfiguration<City>
    {
        public void Configure(EntityTypeBuilder<City> builder)
        {
            var initialDate = new DateTime(2025, 01, 01);

            builder.HasData(
                new City { Id = 1, Name = "Acevedo", DepartmentId = 17, CreatedAt = initialDate },
                new City { Id = 2, Name = "Agrado", DepartmentId = 17, CreatedAt = initialDate },
                new City { Id = 3, Name = "Aipe", DepartmentId = 17, CreatedAt = initialDate },
                new City { Id = 4, Name = "Algeciras", DepartmentId = 17, CreatedAt = initialDate },
                new City { Id = 5, Name = "Altamira", DepartmentId = 17, CreatedAt = initialDate },
                new City { Id = 6, Name = "Baraya", DepartmentId = 17, CreatedAt = initialDate },
                new City { Id = 7, Name = "Campoalegre", DepartmentId = 17, CreatedAt = initialDate },
                new City { Id = 8, Name = "Colombia", DepartmentId = 17, CreatedAt = initialDate },
                new City { Id = 9, Name = "Elías", DepartmentId = 17, CreatedAt = initialDate },
                new City { Id = 10, Name = "Garzón", DepartmentId = 17, CreatedAt = initialDate },
                new City { Id = 11, Name = "Gigante", DepartmentId = 17, CreatedAt = initialDate },
                new City { Id = 12, Name = "Guadalupe", DepartmentId = 17, CreatedAt = initialDate },
                new City { Id = 13, Name = "Hobo", DepartmentId = 17, CreatedAt = initialDate },
                new City { Id = 14, Name = "Iquira", DepartmentId = 17, CreatedAt = initialDate },
                new City { Id = 15, Name = "Isnos", DepartmentId = 17, CreatedAt = initialDate },
                new City { Id = 16, Name = "La Argentina", DepartmentId = 17, CreatedAt = initialDate },
                new City { Id = 17, Name = "La Plata", DepartmentId = 17, CreatedAt = initialDate },
                new City { Id = 18, Name = "Nátaga", DepartmentId = 17, CreatedAt = initialDate },
                new City { Id = 19, Name = "Neiva", DepartmentId = 17, CreatedAt = initialDate },
                new City { Id = 20, Name = "Oporapa", DepartmentId = 17, CreatedAt = initialDate },
                new City { Id = 21, Name = "Paicol", DepartmentId = 17, CreatedAt = initialDate },
                new City { Id = 22, Name = "Palermo", DepartmentId = 17, CreatedAt = initialDate },
                new City { Id = 23, Name = "Palestina", DepartmentId = 17, CreatedAt = initialDate },
                new City { Id = 24, Name = "Pital", DepartmentId = 17, CreatedAt = initialDate },
                new City { Id = 25, Name = "Pitalito", DepartmentId = 17, CreatedAt = initialDate },
                new City { Id = 26, Name = "Rivera", DepartmentId = 17, CreatedAt = initialDate },
                new City { Id = 27, Name = "Saladoblanco", DepartmentId = 17, CreatedAt = initialDate },
                new City { Id = 28, Name = "San Agustín", DepartmentId = 17, CreatedAt = initialDate },
                new City { Id = 29, Name = "Santa María", DepartmentId = 17, CreatedAt = initialDate },
                new City { Id = 30, Name = "Suaza", DepartmentId = 17, CreatedAt = initialDate },
                new City { Id = 31, Name = "Tarqui", DepartmentId = 17, CreatedAt = initialDate },
                new City { Id = 32, Name = "Tello", DepartmentId = 17, CreatedAt = initialDate },
                new City { Id = 33, Name = "Teruel", DepartmentId = 17, CreatedAt = initialDate },
                new City { Id = 34, Name = "Tesalia", DepartmentId = 17, CreatedAt = initialDate },
                new City { Id = 35, Name = "Timaná", DepartmentId = 17, CreatedAt = initialDate },
                new City { Id = 36, Name = "Villavieja", DepartmentId = 17, CreatedAt = initialDate },
                new City { Id = 37, Name = "Yaguará", DepartmentId = 17, CreatedAt = initialDate }
            );
        }

    }
}