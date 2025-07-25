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
                    Description = "Consulta sobre producto X",
                    RequestDate = seedDate,
                    DateTimeAssigned = seedDate.AddDays(2).AddHours(10),
                    CreatedAt = seedDate,
                    Active = true,
                    IsDeleted = false
                },
                new Appointment
                {
                    Id = 2,
                    FullName = "Laura Gómez",
                    Email = "laura.gomez@example.com",
                    Phone = "3109876543",
                    Description = "Revisión de contrato",
                    RequestDate = seedDate.AddDays(1),
                    DateTimeAssigned = seedDate.AddDays(3).AddHours(14),
                    CreatedAt = seedDate,
                    Active = true,
                    IsDeleted = false
                }
            );
        }
    }
}