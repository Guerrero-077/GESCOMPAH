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
                new Form { Id = 1, Name = "Usuarios", Description = "Gestión de usuarios", Route = "/admin/users", IsDeleted = false, Active = true, CreatedAt = seedDate },
                new Form { Id = 2, Name = "Roles", Description = "Gestión de roles", Route = "/admin/roles", IsDeleted = false, Active = true, CreatedAt = seedDate },
                new Form { Id = 3, Name = "Locales", Description = "Gestión de locales", Route = "/locales", IsDeleted = false, Active = true, CreatedAt = seedDate }
            );
        }
    }
}