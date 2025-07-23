using Entity.Domain.Models.Implements.Location;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Entity.Infrastructure.Configurations.Locaton
{
    public class CityConfiguration : IEntityTypeConfiguration<City>
    {
        public void Configure(EntityTypeBuilder<City> builder)
        {
            builder.ToTable("Cities");

            builder.HasKey(c => c.Id);

            builder.Property(c => c.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.HasIndex(c => new { c.Name, c.DepartmentId }) // Ciudad única por departamento
                .IsUnique();

            builder.HasOne(c => c.Department)
                .WithMany(d => d.Cities)
                .HasForeignKey(c => c.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict); // Opción defensiva, evita borrado en cascada

            builder.Navigation(c => c.Department).AutoInclude(); // opcional si quieres eager loading
        }
    }
}
