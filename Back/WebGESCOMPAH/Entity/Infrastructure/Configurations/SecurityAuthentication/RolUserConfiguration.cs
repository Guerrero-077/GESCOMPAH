using Entity.Domain.Models.Implements.SecurityAuthentication;
using Entity.Infrastructure.Configurations.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Entity.Infrastructure.Configurations.SecurityAuthentication
{
    public class RolUserConfiguration : BaseModelConfiguration<RolUser>
    {
        public override void Configure(EntityTypeBuilder<RolUser> builder)
        {
            base.Configure(builder); // Aplica: Id, CreatedAt, Active, IsDeleted

            builder.ToTable("RolUsers");

            builder.HasOne(ru => ru.User)
                .WithMany(u => u.RolUsers)
                .HasForeignKey(ru => ru.UserId)
                .OnDelete(DeleteBehavior.Cascade); // O Restrict según tu lógica de negocio

            builder.HasOne(ru => ru.Rol)
                .WithMany(r => r.RolUsers)
                .HasForeignKey(ru => ru.RolId)
                .OnDelete(DeleteBehavior.Restrict); // Evita que al borrar un rol se borren usuarios

            // Evita duplicados: un mismo usuario no puede tener el mismo rol más de una vez
            builder.HasIndex(ru => new { ru.UserId, ru.RolId })
                   .IsUnique();
        }
    }
}
