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

            builder.Property(e => e.Active).IsRequired();
            builder.Property(e => e.IsDeleted).IsRequired();

            // Elimina configuración específica de motor para CreatedAt
            builder.Property(e => e.CreatedAt)
                   .IsRequired(); // Solo requerir, sin tipo ni default SQL


            // Índice compuesto para ordenar “lo último primero”
            builder.HasIndex(e => new { e.CreatedAt, e.Id })
                   .IsDescending(true, true); // EF Core 7+ soporta índices descendentes
        }
    }
}
