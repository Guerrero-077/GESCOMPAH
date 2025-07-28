using Entity.Domain.Models.Implements.Business;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Entity.Infrastructure.DataInit.Business
{
    public class AppointmentSeeder : IEntityTypeConfiguration<Appointment>
    {
        public void Configure(EntityTypeBuilder<Appointment> builder)
        {
            var seedDate = new DateTime(2025, 01, 01);

            builder.HasData(
                new Appointment
                {
                    Id = 1,
                    FullName = "Juan Pérez",
                    Email = "juan.perez@example.com",
                    Phone = "3001234567",
                    Description = "Solicitud para conocer el local",
                    RequestDate = seedDate,
                    DateTimeAssigned = seedDate.AddDays(3).AddHours(10),
                    EstablishmentId = 1,
                    CreatedAt = seedDate,
                    Active = true,
                    IsDeleted = false
                },
                new Appointment
                {
                    Id = 2,
                    FullName = "María Gómez",
                    Email = "maria.gomez@example.com",
                    Phone = "3019876543",
                    Description = "Revisión de contrato anterior",
                    RequestDate = seedDate.AddDays(1),
                    DateTimeAssigned = seedDate.AddDays(4).AddHours(11),
                    EstablishmentId = 2,
                    CreatedAt = seedDate.AddDays(1),
                    Active = true,
                    IsDeleted = false
                },
                new Appointment
                {
                    Id = 3,
                    FullName = "Carlos Ramírez",
                    Email = "carlos.ramirez@example.com",
                    Phone = "3021122334",
                    Description = "Consulta sobre requisitos para arriendo",
                    RequestDate = seedDate.AddDays(2),
                    DateTimeAssigned = seedDate.AddDays(5).AddHours(9),
                    EstablishmentId = 3,
                    CreatedAt = seedDate.AddDays(2),
                    Active = true,
                    IsDeleted = false
                }
            );
        }
    }
}
