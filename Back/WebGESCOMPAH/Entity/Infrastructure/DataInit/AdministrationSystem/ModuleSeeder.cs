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
                new Module { 
                    Id = 1, 
                    Name = "Administración", 
                    Description = "Módulo de gestión administrativa", 
                    Icon = "shield-lock", 
                    IsDeleted = false, 
                    Active = true, 
                    CreatedAt = seedDate 
                },
                new Module { 
                    Id = 2, 
                    Name = "Locales", 
                    Description = "Módulo de gestión de locales municipales", 
                    Icon = "store-front", 
                    IsDeleted = false, 
                    Active = true, 
                    CreatedAt = seedDate 
                }
            );
        }
    }
}