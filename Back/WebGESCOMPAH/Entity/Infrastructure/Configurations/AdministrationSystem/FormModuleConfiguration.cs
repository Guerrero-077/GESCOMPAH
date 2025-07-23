using Entity.Domain.Models.Implements.AdministrationSystem;
using Entity.Infrastructure.Configurations.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Entity.Infrastructure.Configurations.AdministrationSystem
{
    public class FormModuleConfiguration : BaseModelConfiguration<FormModule>
    {
        public override void Configure(EntityTypeBuilder<FormModule> builder)
        {
            base.Configure(builder); // Hereda configuración de BaseModel: Id, Active, CreatedAt, etc.

            builder.ToTable("FormModules");

            builder.HasOne(fm => fm.Form)
                .WithMany(f => f.FormModules)
                .HasForeignKey(fm => fm.FormId)
                .OnDelete(DeleteBehavior.Restrict); // O Cascade, según tu lógica de negocio

            builder.HasOne(fm => fm.Module)
                .WithMany(m => m.FormModules)
                .HasForeignKey(fm => fm.ModuleId)
                .OnDelete(DeleteBehavior.Restrict);

            // Unicidad: un mismo Form no puede tener el mismo Module dos veces
            builder.HasIndex(fm => new { fm.FormId, fm.ModuleId })
                   .IsUnique();
        }
    }

}