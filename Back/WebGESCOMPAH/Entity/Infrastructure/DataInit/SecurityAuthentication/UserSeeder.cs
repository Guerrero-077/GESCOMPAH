using Entity.Domain.Models.Implements.SecurityAuthentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Entity.Infrastructure.DataInit.SecurityAuthentication
{
    public class UserSeeder : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            var seedDate = new DateTime(2025, 01, 01);

            builder.HasData(

                new User
                {
                    Id = 1,
                    Email = "admin@gescomph.com",
                    Password = "admin123", // Hashealo si puedes
                    IsDeleted = false,
                    Active = true,
                    CreatedAt = seedDate,
                    PersonId = 1
                }
            );

        }
    }
}
