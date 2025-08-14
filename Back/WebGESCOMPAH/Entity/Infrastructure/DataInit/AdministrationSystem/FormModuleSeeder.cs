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
                new FormModule { 
                    Id = 1, 
                    FormId = 1, 
                    ModuleId = 1,  
                    IsDeleted = false, 
                    Active = true, 
                    CreatedAt = seedDate 
                }, // Home -> DashBoard
                new FormModule { 
                    Id = 2, 
                    FormId = 2, 
                    ModuleId = 2,  
                    IsDeleted = false, 
                    Active = true, 
                    CreatedAt = seedDate 
                }, // Estblecimientos -> Establecimientos
                new FormModule { 
                    Id = 3, 
                    FormId = 3, 
                    ModuleId = 3,  
                    IsDeleted = false, 
                    Active = true, 
                    CreatedAt = seedDate 
                },  // Arrendatarios -> Arrendatarios
                new FormModule { 
                    Id = 4, 
                    FormId = 5, 
                    ModuleId = 4,  
                    IsDeleted = false, 
                    Active = true, 
                    CreatedAt = seedDate 
                },  // Roles -> Security
                new FormModule { 
                    Id = 5, 
                    FormId = 6, 
                    ModuleId = 4,  
                    IsDeleted = false, 
                    Active = true, 
                    CreatedAt = seedDate 
                },  // forms -> Security
                new FormModule { 
                    Id = 6, 
                    FormId = 7, 
                    ModuleId = 4,  
                    IsDeleted = false, 
                    Active = true, 
                    CreatedAt = seedDate 
                },  // Modules -> Security
                new FormModule { 
                    Id = 7, 
                    FormId = 8, 
                    ModuleId = 4,  
                    IsDeleted = false, 
                    Active = true, 
                    CreatedAt = seedDate 
                },  // Permissions -> Security
                new FormModule { 
                    Id = 8, 
                    FormId = 4, 
                    ModuleId = 4,  
                    IsDeleted = false, 
                    Active = true, 
                    CreatedAt = seedDate 
                },  // Modulo de seguridad -> Security


                new FormModule { 
                    Id = 9, 
                    FormId = 9, 
                    ModuleId = 5,  
                    IsDeleted = false, 
                    Active = true, 
                    CreatedAt = seedDate 
                }  // Config -> Config
            );
        }
    }
}