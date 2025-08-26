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
                    // Contraseña segura: "admin123"
                    Password = "AQAAAAEAACcQAAAAEK1QvWufDHBzB3acG5GKxdQTabH8BhbyLLyyZHo4WoOEvRYijXcOtRqsb3OeOpoGqw==",
                    IsDeleted = false,
                    Active = true,
                    CreatedAt = seedDate,
                    PersonId = 1
                },
                new User
                {
                    Id = 2,
                    Email = "usuario@gescomph.com",
                    // Usuario1.
                    Password = "AQAAAAIAAYagAAAAEGNtpwDVV/mpIlUqi5xrPjpvzCejMXq142erkCJONaKJSiXb73eZm1tPxzj+2RvBXw==",
                    IsDeleted = false,
                    Active = true,
                    CreatedAt = seedDate,
                    PersonId = 2
                }
            );
        }
    }
}
