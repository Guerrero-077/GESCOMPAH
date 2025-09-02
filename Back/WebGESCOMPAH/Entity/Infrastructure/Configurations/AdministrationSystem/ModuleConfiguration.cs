using Entity.Domain.Models.Implements.AdministrationSystem;
using Entity.Infrastructure.Configurations.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Entity.Infrastructure.Configurations.AdministrationSystem
{
    public class ModuleConfiguration : BaseModelGenericConfiguration<Module>
    {
        public override void Configure(EntityTypeBuilder<Module> builder)
        {
            base.Configure(builder); // Aplica: Id, Active, IsDeleted, CreatedAt, Name, Description

            builder.ToTable("Modules");

            builder.Property(m => m.Icon)
                .IsRequired()
                .HasMaxLength(100); // Ajusta este valor si tienes un límite definido (e.g., nombre de clase CSS o URL)

            builder.HasMany(m => m.FormModules)
                .WithOne(fm => fm.Module)
                .HasForeignKey(fm => fm.ModuleId)
                .OnDelete(DeleteBehavior.Restrict); // Puedes ajustar a Cascade si aplica a tu negocio
        }
    }
}