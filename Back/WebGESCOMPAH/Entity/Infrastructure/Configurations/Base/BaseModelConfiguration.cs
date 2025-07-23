using Entity.Domain.Models.ModelBase;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Entity.Infrastructure.Configurations.Base
{
    public abstract class BaseModelConfiguration<T> : IEntityTypeConfiguration<T> where T : BaseModel
    {
        public virtual void Configure(EntityTypeBuilder<T> builder)
        {
            builder.HasKey(e => e.Id);

            builder.Property(e => e.Active)
                   .IsRequired();

            builder.Property(e => e.IsDeleted)
                   .IsRequired();

            builder.Property(e => e.CreatedAt)
                   .HasDefaultValueSql("GETDATE()") // o NOW() en PostgreSQL
                   .ValueGeneratedOnAdd();
        }
    }
}