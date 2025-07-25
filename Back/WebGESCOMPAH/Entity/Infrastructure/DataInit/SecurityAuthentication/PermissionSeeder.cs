using Entity.Domain.Models.Implements.SecurityAuthentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Entity.Infrastructure.DataInit.SecurityAuthentication
{
    public class PermissionSeeder : IEntityTypeConfiguration<Permission>
    {
        public void Configure(EntityTypeBuilder<Permission> builder)
        {
            var seedDate = new DateTime(2025, 01, 01);

            builder.HasData(
                new Permission { 
                    Id = 1, 
                    Name = "Ver", 
                    Description = "Permite ver registros",  
                    IsDeleted = false, 
                    Active = true,
                    CreatedAt = seedDate 
                },
                new Permission { 
                    Id = 2, 
                    Name = "Crear", 
                    Description = "Permite crear registros",  
                    IsDeleted = false, 
                    Active = true,
                    CreatedAt = seedDate
                },
                new Permission { 
                    Id = 3, 
                    Name = "Editar", 
                    Description = "Permite editar registros",  
                    IsDeleted = false, 
                    Active = true,
                    CreatedAt = seedDate 
                },
                new Permission { 
                    Id = 4, 
                    Name = "Eliminar", 
                    Description = "Permite eliminar registros",
                    IsDeleted = false, 
                    Active = true,
                    CreatedAt = seedDate
                }
            );
        }
    }
}