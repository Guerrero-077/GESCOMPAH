using Entity.Domain.Models.Implements.Location;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Entity.Infrastructure.DataInit.Location
{
    public class DepartmentSeeder : IEntityTypeConfiguration<Department>
    {
        public void Configure(EntityTypeBuilder<Department> builder)
        {
            var seedDate = new DateTime(2025, 01, 01, 0, 0, 0, DateTimeKind.Utc);

            builder.HasData(
                new Department { Id = 1, Name = "Amazonas", CreatedAt = seedDate },
                new Department { Id = 2, Name = "Antioquia", CreatedAt = seedDate },
                new Department { Id = 3, Name = "Arauca", CreatedAt = seedDate },
                new Department { Id = 4, Name = "Atlántico", CreatedAt = seedDate },
                new Department { Id = 5, Name = "Bolívar", CreatedAt = seedDate },
                new Department { Id = 6, Name = "Boyacá", CreatedAt = seedDate },
                new Department { Id = 7, Name = "Caldas", CreatedAt = seedDate },
                new Department { Id = 8, Name = "Caquetá", CreatedAt = seedDate },
                new Department { Id = 9, Name = "Casanare", CreatedAt = seedDate },
                new Department { Id = 10, Name = "Cauca", CreatedAt = seedDate },
                new Department { Id = 11, Name = "Cesar", CreatedAt = seedDate },
                new Department { Id = 12, Name = "Chocó", CreatedAt = seedDate },
                new Department { Id = 13, Name = "Córdoba", CreatedAt = seedDate },
                new Department { Id = 14, Name = "Cundinamarca", CreatedAt = seedDate },
                new Department { Id = 15, Name = "Guainía", CreatedAt = seedDate },
                new Department { Id = 16, Name = "Guaviare", CreatedAt = seedDate },
                new Department { Id = 17, Name = "Huila", CreatedAt = seedDate },
                new Department { Id = 18, Name = "La Guajira", CreatedAt = seedDate },
                new Department { Id = 19, Name = "Magdalena", CreatedAt = seedDate },
                new Department { Id = 20, Name = "Meta", CreatedAt = seedDate },
                new Department { Id = 21, Name = "Nariño", CreatedAt = seedDate },
                new Department { Id = 22, Name = "Norte de Santander", CreatedAt = seedDate },
                new Department { Id = 23, Name = "Putumayo", CreatedAt = seedDate },
                new Department { Id = 24, Name = "Quindío", CreatedAt = seedDate },
                new Department { Id = 25, Name = "Risaralda", CreatedAt = seedDate },
                new Department { Id = 26, Name = "San Andrés y Providencia", CreatedAt = seedDate },
                new Department { Id = 27, Name = "Santander", CreatedAt = seedDate },
                new Department { Id = 28, Name = "Sucre", CreatedAt = seedDate },
                new Department { Id = 29, Name = "Tolima", CreatedAt = seedDate },
                new Department { Id = 30, Name = "Valle del Cauca", CreatedAt = seedDate },
                new Department { Id = 31, Name = "Vaupés", CreatedAt = seedDate },
                new Department { Id = 32, Name = "Vichada", CreatedAt = seedDate }
            );
        }


    }
}