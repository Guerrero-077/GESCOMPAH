using Entity.Domain.Models.Implements.SecurityAuthentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Entity.Infrastructure.DataInit.SecurityAuthentication
{
    public class RolUserSeeder : IEntityTypeConfiguration<RolUser>
    {

        public void Configure(EntityTypeBuilder<RolUser> builder)
        {
            var seedDate = new DateTime(2025, 01, 01);

            builder.HasData(
                 new RolUser { Id = 1, UserId = 1, RolId = 1, IsDeleted = false, Active = true, CreatedAt = seedDate },
                 new RolUser { Id = 2, UserId = 2, RolId = 2, IsDeleted = false, Active = true, CreatedAt = seedDate }
                );
        }
    }
}
