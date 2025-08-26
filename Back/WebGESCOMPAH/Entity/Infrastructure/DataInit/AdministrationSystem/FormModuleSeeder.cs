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
                // Panel Principal
                new FormModule { Id = 1, FormId = 1, ModuleId = 1, IsDeleted = false, Active = true, CreatedAt = seedDate },

                // Establecimientos
                new FormModule { Id = 2, FormId = 2, ModuleId = 2, IsDeleted = false, Active = true, CreatedAt = seedDate },

                // Arrendatarios
                new FormModule { Id = 3, FormId = 3, ModuleId = 3, IsDeleted = false, Active = true, CreatedAt = seedDate },

                // Contratos
                new FormModule { Id = 4, FormId = 4, ModuleId = 4, IsDeleted = false, Active = true, CreatedAt = seedDate },

                // Gestión de Citas
                new FormModule { Id = 5, FormId = 5, ModuleId = 5, IsDeleted = false, Active = true, CreatedAt = seedDate },

                // Seguridad
                new FormModule { Id = 6, FormId = 6, ModuleId = 6, IsDeleted = false, Active = true, CreatedAt = seedDate }, // Modelos de Seguridad
                new FormModule { Id = 7, FormId = 7, ModuleId = 6, IsDeleted = false, Active = true, CreatedAt = seedDate }, // Roles
                new FormModule { Id = 8, FormId = 8, ModuleId = 6, IsDeleted = false, Active = true, CreatedAt = seedDate }, // Formularios
                new FormModule { Id = 9, FormId = 9, ModuleId = 6, IsDeleted = false, Active = true, CreatedAt = seedDate }, // Módulos
                new FormModule { Id = 10, FormId = 10, ModuleId = 6, IsDeleted = false, Active = true, CreatedAt = seedDate }, // Permisos

                // Configuración
                new FormModule { Id = 11, FormId = 11, ModuleId = 7, IsDeleted = false, Active = true, CreatedAt = seedDate }
            );
        }
    }
}
