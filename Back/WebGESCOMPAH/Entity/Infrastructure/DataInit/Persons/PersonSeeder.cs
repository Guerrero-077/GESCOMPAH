using Entity.Domain.Models.Implements.Persons;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                }
                
            );
        }
    }
};
