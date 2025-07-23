using Entity.Domain.Models.ModelBase;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Entity.Infrastructure.Configurations.Base
{
    public abstract class BaseModelGenericConfiguration<T> : BaseModelConfiguration<T>
        where T : BaseModelGeneric
    {
        public override void Configure(EntityTypeBuilder<T> builder)
        {
            base.Configure(builder);

            builder.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.HasIndex(e => e.Name).IsUnique();

            builder.Property(e => e.Description)
                .HasMaxLength(300);
        }
    }
}

