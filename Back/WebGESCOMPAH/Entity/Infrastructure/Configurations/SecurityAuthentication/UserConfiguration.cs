using Entity.Domain.Models.Implements.SecurityAuthentication;
using Entity.Infrastructure.Configurations.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Entity.Infrastructure.Configurations.SecurityAuthentication
{
    public class UserConfiguration : BaseModelConfiguration<User>
    {
        public override void Configure(EntityTypeBuilder<User> builder)
        {
            base.Configure(builder); // Hereda Id, CreatedAt, Active, IsDeleted

            builder.ToTable("Users");

            // Email obligatorio, único, con longitud controlada
            builder.Property(u => u.Email)
                .IsRequired()
                .HasMaxLength(150);

            builder.HasIndex(u => u.Email)
                .IsUnique();

            // Password obligatorio, con longitud mínima
            builder.Property(u => u.Password)
                .IsRequired()
                .HasMaxLength(256); // Asume hash largo como bcrypt/argon2

            // Relación 1:1 con Person
            builder.HasOne(u => u.Person)
                .WithOne(p => p.User)
                .HasForeignKey<User>(u => u.PersonId)
                .OnDelete(DeleteBehavior.Restrict); // Evita que borrar el User borre la Persona

            // Relación 1:N con RolUsers
            builder.HasMany(u => u.RolUsers)
                .WithOne(ru => ru.User)
                .HasForeignKey(ru => ru.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}