using Entity.Domain.Models.Implements.Location;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Entity.Infrastructure.DataInit.Location
{
    public class DepartmentSeeder : IEntityTypeConfiguration<Department>
    {
        public void Configure(EntityTypeBuilder<Department> builder)
        {
            var initialDate = new DateTime(2024, 01, 01);

            builder.HasData(
                new Department { Id = 1, Name = "Amazonas", CreatedAt = initialDate },
                new Department { Id = 2, Name = "Antioquia", CreatedAt = initialDate },
                new Department { Id = 3, Name = "Arauca", CreatedAt = initialDate },
                new Department { Id = 4, Name = "Atlántico", CreatedAt = initialDate },
                new Department { Id = 5, Name = "Bolívar", CreatedAt = initialDate },
                new Department { Id = 6, Name = "Boyacá", CreatedAt = initialDate },
                new Department { Id = 7, Name = "Caldas", CreatedAt = initialDate },
                new Department { Id = 8, Name = "Caquetá", CreatedAt = initialDate },
                new Department { Id = 9, Name = "Casanare", CreatedAt = initialDate },
                new Department { Id = 10, Name = "Cauca", CreatedAt = initialDate },
                new Department { Id = 11, Name = "Cesar", CreatedAt = initialDate },
                new Department { Id = 12, Name = "Chocó", CreatedAt = initialDate },
                new Department { Id = 13, Name = "Córdoba", CreatedAt = initialDate },
                new Department { Id = 14, Name = "Cundinamarca", CreatedAt = initialDate },
                new Department { Id = 15, Name = "Guainía", CreatedAt = initialDate },
                new Department { Id = 16, Name = "Guaviare", CreatedAt = initialDate },
                new Department { Id = 17, Name = "Huila", CreatedAt = initialDate },
                new Department { Id = 18, Name = "La Guajira", CreatedAt = initialDate },
                new Department { Id = 19, Name = "Magdalena", CreatedAt = initialDate },
                new Department { Id = 20, Name = "Meta", CreatedAt = initialDate },
                new Department { Id = 21, Name = "Nariño", CreatedAt = initialDate },
                new Department { Id = 22, Name = "Norte de Santander", CreatedAt = initialDate },
                new Department { Id = 23, Name = "Putumayo", CreatedAt = initialDate },
                new Department { Id = 24, Name = "Quindío", CreatedAt = initialDate },
                new Department { Id = 25, Name = "Risaralda", CreatedAt = initialDate },
                new Department { Id = 26, Name = "San Andrés y Providencia", CreatedAt = initialDate },
                new Department { Id = 27, Name = "Santander", CreatedAt = initialDate },
                new Department { Id = 28, Name = "Sucre", CreatedAt = initialDate },
                new Department { Id = 29, Name = "Tolima", CreatedAt = initialDate },
                new Department { Id = 30, Name = "Valle del Cauca", CreatedAt = initialDate },
                new Department { Id = 31, Name = "Vaupés", CreatedAt = initialDate },
                new Department { Id = 32, Name = "Vichada", CreatedAt = initialDate }
            );
        }


    }
}