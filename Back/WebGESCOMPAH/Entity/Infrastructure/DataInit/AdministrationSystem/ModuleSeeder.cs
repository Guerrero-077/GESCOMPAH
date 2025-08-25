using Entity.Domain.Models.Implements.AdministrationSystem;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Entity.Infrastructure.DataInit.AdministrationSystem
{
    public class ModuleSeeder : IEntityTypeConfiguration<Module>
    {
        public void Configure(EntityTypeBuilder<Module> builder)
        {
            var seedDate = new DateTime(2025, 01, 01);

            builder.HasData(
                new Module { Id = 1, Name = "Panel Principal", Description = "Panel de control principal", Icon = "home", IsDeleted = false, Active = true, CreatedAt = seedDate },

                new Module { Id = 2, Name = "Establecimientos", Description = "Gestión de establecimientos", Icon = "store", IsDeleted = false, Active = true, CreatedAt = seedDate },

                new Module { Id = 3, Name = "Arrendatarios", Description = "Gestión de arrendatarios", Icon = "people", IsDeleted = false, Active = true, CreatedAt = seedDate },

                new Module { Id = 4, Name = "Contratos", Description = "Gestión de contratos", Icon = "description", IsDeleted = false, Active = true, CreatedAt = seedDate },

                new Module { Id = 5, Name = "Gestión de Citas", Description = "Gestión de citas", Icon = "event_note", IsDeleted = false, Active = true, CreatedAt = seedDate },

                new Module { Id = 6, Name = "Seguridad", Description = "Gestión de seguridad y permisos", Icon = "security", IsDeleted = false, Active = true, CreatedAt = seedDate },

                new Module { Id = 7, Name = "Configuración", Description = "Configuración general del sistema", Icon = "settings", IsDeleted = false, Active = true, CreatedAt = seedDate }
            );
        }
    }
}
