using Entity.Domain.Models.Implements.Persons;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Entity.Infrastructure.DataInit.Persons
{
    public class PersonSeeder : IEntityTypeConfiguration<Person>
    {
        public void Configure(EntityTypeBuilder<Person> builder)
        {

            var seedDate = new DateTime(2025, 01, 01);

            builder.HasData(
                new Person
                {
                    Id = 1,
                    FirstName = "Administrador",
                    LastName = "General",
                    Document = "123456789",
                    Address = "Calle Principal 123",
                    Phone = "3000000000",
                    CityId = 1,
                    IsDeleted = false,
                    Active = true,
                    CreatedAt = seedDate
                },
                new Person
                {
                    Id = 2,
                    FirstName = "Usuario",
                    LastName = "General",
                    Document = "1000000000",
                    Address = "Calle Principal 123",
                    Phone = "3000000000",
                    CityId = 1,
                    IsDeleted = false,
                    Active = true,
                    CreatedAt = seedDate
                }
                
            );
        }
    }
};
