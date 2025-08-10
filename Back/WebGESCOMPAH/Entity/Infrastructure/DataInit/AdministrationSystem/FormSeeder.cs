using Entity.Domain.Models.Implements.AdministrationSystem;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Entity.Infrastructure.DataInit.AdministrationSystem
{
    public class FormSeeder : IEntityTypeConfiguration<Form>
    {
        public void Configure(EntityTypeBuilder<Form> builder)
        {
            var seedDate = new DateTime(2025, 01, 01);

            builder.HasData(
                new Form { Id = 1, Name = "Vista General", Description = "Vista general del dashboard", Route = "dashboard", IsDeleted = false, Active = true, CreatedAt = seedDate },
                new Form { Id = 2, Name = "Lista de Establecimientos", Description = "Ver y gestionar establecimientos", Route = "establishments/main", IsDeleted = false, Active = true, CreatedAt = seedDate },
                new Form { Id = 3, Name = "Arrendatarios", Description = "Gestión de arrendatarios", Route = "tenants", IsDeleted = false, Active = true, CreatedAt = seedDate },
                new Form { Id = 4, Name = "Modelos de Seguridad", Description = "Configuración de modelos de seguridad", Route = "security/main", IsDeleted = false, Active = true, CreatedAt = seedDate },
                new Form { Id = 5, Name = "Configuración Principal", Description = "Ajustes principales del sistema", Route = "settings/main", IsDeleted = false, Active = true, CreatedAt = seedDate }
            );
        }
    }
}