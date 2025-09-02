using Entity.Domain.Models.Implements.AdministrationSystem;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Entity.Infrastructure.DataInit.AdministrationSystem
{
    public class FormSeeder : IEntityTypeConfiguration<Form>
    {
        public void Configure(EntityTypeBuilder<Form> builder)
        {
            var seedDate = new DateTime(2025, 01, 01, 0, 0, 0, DateTimeKind.Utc);
            builder.HasData(
                new Form { Id = 1, Name = "Inicio", Description = "Vista general de la pagina principal", Route = "dashboard", IsDeleted = false, Active = true, CreatedAt = seedDate },

                new Form { Id = 2, Name = "Establecimientos", Description = "Ver y gestionar establecimientos", Route = "establishments/main", IsDeleted = false, Active = true, CreatedAt = seedDate },

                new Form { Id = 3, Name = "Arrendatarios", Description = "Gestión de arrendatarios", Route = "tenants", IsDeleted = false, Active = true, CreatedAt = seedDate },
               
                new Form { Id = 4, Name = "Contratos", Description = "Gestión de contratos", Route = "contracts", IsDeleted = false, Active = true, CreatedAt = seedDate },

                new Form { Id = 5, Name = "Citas", Description = "Gestión de Citas", Route = "appointment", IsDeleted = false, Active = true, CreatedAt = seedDate },

                new Form { Id = 6, Name = "Modelos de Seguridad", Description = "Configuración de modelos de seguridad", Route = "security/main", IsDeleted = false, Active = true, CreatedAt = seedDate }
                ,
                new Form { Id = 7, Name = "Roles", Description = "Gestión de roles", Route = "security/roles", IsDeleted = false, Active = true, CreatedAt = seedDate },

                new Form { Id = 8, Name = "Formularios", Description = "Gestión de Formularios", Route = "security/forms", IsDeleted = false, Active = true, CreatedAt = seedDate },

                new Form { Id = 9, Name = "Modulos", Description = "Gestión de módulos", Route = "security/modules", IsDeleted = false, Active = true, CreatedAt = seedDate },

                new Form { Id = 10, Name = "Permisos", Description = "Gestión de Permisos", Route = "security/permissions", IsDeleted = false, Active = true, CreatedAt = seedDate },

                new Form { Id = 11, Name = "Configuración Principal", Description = "Ajustes principales del sistema", Route = "settings/main", IsDeleted = false, Active = true, CreatedAt = seedDate }
            );

        }
    }
}