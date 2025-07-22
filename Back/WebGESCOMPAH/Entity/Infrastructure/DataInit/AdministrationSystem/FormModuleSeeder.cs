using Entity.Domain.Models.Implements.AdministrationSystem;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Entity.Infrastructure.DataInit.AdministrationSystem
{
    public class FormModuleSeeder : IEntityTypeConfiguration<FormModule>
    {
        public void Configure(EntityTypeBuilder<FormModule> builder)
        {
            var seedDate = new DateTime(2025, 01, 01);


            builder.HasData(
                // Asignamos cada formulario a su módulo correspondiente
                new FormModule { Id = 1, FormId = 1, ModuleId = 1,  IsDeleted = false, Active = true, CreatedAt = seedDate }, // Usuarios -> Administración
                new FormModule { Id = 2, FormId = 2, ModuleId = 1,  IsDeleted = false, Active = true, CreatedAt = seedDate }, // Roles -> Administración
                new FormModule { Id = 3, FormId = 3, ModuleId = 2, IsDeleted = false, Active = true, CreatedAt = seedDate }  // Locales -> Locales
            );
        }
    }
}