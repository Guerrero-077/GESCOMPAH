using Entity.Domain.Models.Implements.AdministrationSystem;
using Entity.Infrastructure.Configurations.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Entity.Infrastructure.Configurations.AdministrationSystem
{
    public class FormConfiguration : BaseModelGenericConfiguration<Form>
    {
        public override void Configure(EntityTypeBuilder<Form> builder)
        {
            base.Configure(builder); // Aplica configuración heredada: Id, Active, Name, Description

            builder.ToTable("Forms");

            builder.Property(f => f.Route)
                .IsRequired()
                .HasMaxLength(200); // Puedes ajustar según los requerimientos del sistema

            // Relación con RolFormPermission (1:n)
            builder.HasMany(f => f.RolFormPermissions)
                .WithOne(rfp => rfp.Form)
                .HasForeignKey(rfp => rfp.FormId)
                .OnDelete(DeleteBehavior.Restrict); // o Cascade si quieres borrar permisos con el form

            // Relación con FormModule (1:n)
            builder.HasMany(f => f.FormModules)
                .WithOne(fm => fm.Form)
                .HasForeignKey(fm => fm.FormId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
