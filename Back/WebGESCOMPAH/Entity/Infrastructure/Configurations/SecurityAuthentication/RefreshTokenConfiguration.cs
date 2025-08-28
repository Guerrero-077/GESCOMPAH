using Entity.Domain.Models.Implements.SecurityAuthentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Entity.Infrastructure.Configurations.SecurityAuthentication
{
    public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
    {
        public void Configure(EntityTypeBuilder<RefreshToken> builder)
        {
            builder.ToTable("RefreshToken");

            builder.HasKey(e => e.Id);

            builder.Property(e => e.TokenHash)
                .IsRequired()
                .HasMaxLength(128);

            builder.HasIndex(e => e.TokenHash).IsUnique(false);
            builder.HasIndex(e => e.UserId);
        }
    }
}
