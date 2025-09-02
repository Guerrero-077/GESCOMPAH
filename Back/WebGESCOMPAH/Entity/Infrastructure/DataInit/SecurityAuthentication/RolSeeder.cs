using Entity.Domain.Models.Implements.SecurityAuthentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Entity.Infrastructure.DataInit.SecurityAuthentication
{
    public class RolSeeder : IEntityTypeConfiguration<Rol>
    {
        public void Configure(EntityTypeBuilder<Rol> builder)
        {
            var seedDate = new DateTime(2025, 01, 01, 0, 0, 0, DateTimeKind.Utc);
            builder.HasData(
                 new Rol { Id = 1, Name = "Administrador", Description="Rol Con permisos Administrativos", IsDeleted = false, Active = true, CreatedAt = seedDate },
                new Rol { Id = 2, Name = "Arrendador", Description="Rol con permisos de arrendador", IsDeleted = false, Active = true,  CreatedAt = seedDate }

             );
        }
    }
}
