using Entity.Domain.Models.Implements.Persons;
using Entity.Domain.Models.Implements.SecurityAuthentication;
using Entity.Infrastructure.Configurations.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Entity.Infrastructure.Configurations.Persons
{
    public class PersonConfiguration : BaseModelConfiguration<Person>
    {
        public override void Configure(EntityTypeBuilder<Person> builder)
        {
            base.Configure(builder); // ← aplica configuración común de BaseModel

            builder.ToTable("Persons");

            builder.Property(p => p.FirstName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(p => p.LastName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(p => p.Document)
                .HasMaxLength(20);

            builder.HasIndex(p => p.Document)
                .IsUnique()
                .HasFilter("[Document] IS NOT NULL"); // PostgreSQL: "WHERE Document IS NOT NULL"

            builder.Property(p => p.Address)
                .HasMaxLength(200);

            builder.Property(p => p.Phone)
                .HasMaxLength(20);

            builder.HasOne(p => p.City)
                .WithMany(c => c.People)
                .HasForeignKey(p => p.CityId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(p => p.User)
                .WithOne(u => u.Person)
                .HasForeignKey<User>(u => u.PersonId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}