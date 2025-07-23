using Entity.Domain.Models.Implements.SecurityAuthentication;
using Entity.Infrastructure.Configurations.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Entity.Infrastructure.Configurations.SecurityAuthentication
{
    public class RolFormPermissionConfiguration : BaseModelConfiguration<RolFormPermission>
    {
        public override void Configure(EntityTypeBuilder<RolFormPermission> builder)
        {
            base.Configure(builder); // Hereda configuración común: Id, Active, IsDeleted, CreatedAt

            builder.ToTable("RolFormPermissions");

            builder.HasOne(rfp => rfp.Rol)
                .WithMany(r => r.RolFormPermissions)
                .HasForeignKey(rfp => rfp.RolId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(rfp => rfp.Form)
                .WithMany(f => f.RolFormPermissions)
                .HasForeignKey(rfp => rfp.FormId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(rfp => rfp.Permission)
                .WithMany(p => p.RolFormPermissions)
                .HasForeignKey(rfp => rfp.PermissionId)
                .OnDelete(DeleteBehavior.Restrict); // Si quieres evitar borrar permisos en cascada

            // Evitar duplicados: una combinación de Rol, Form y Permission debe ser única
            builder.HasIndex(rfp => new { rfp.RolId, rfp.FormId, rfp.PermissionId })
                   .IsUnique();
        }
    }
}
