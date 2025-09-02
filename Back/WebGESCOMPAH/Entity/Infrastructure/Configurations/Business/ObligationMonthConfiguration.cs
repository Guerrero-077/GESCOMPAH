using Entity.Domain.Models.Implements.Business;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Entity.Infrastructure.Configurations.Business
{
    public class ObligationMonthConfiguration : IEntityTypeConfiguration<ObligationMonth>
    {
        public void Configure(EntityTypeBuilder<ObligationMonth> builder)
        {
            builder.ToTable("ObligationMonths");

            builder.HasKey(o => o.Id);

            builder.Property(o => o.Status)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(o => o.UvtQtyApplied)
                .HasPrecision(18, 4);

            builder.Property(o => o.UvtValueApplied)
                .HasPrecision(18, 4);

            builder.Property(o => o.VatRateApplied)
                .HasPrecision(5, 4); // ejemplo: 0.1900 para 19%

            builder.Property(o => o.BaseAmount)
                .HasPrecision(18, 4);

            builder.Property(o => o.VatAmount)
                .HasPrecision(18, 4);

            builder.Property(o => o.TotalAmount)
                .HasPrecision(18, 4);

            builder.Property(o => o.LateAmount)
                .HasPrecision(18, 4);

            builder.Property(o => o.Locked)
                .IsRequired();

            builder.Property(o => o.DueDate).HasColumnType("timestamp");

            builder.HasOne(o => o.Contract)
                .WithMany(c => c.ObligationMonths) // <-- asegúrate que exista esta navegación en Contract
                .HasForeignKey(o => o.ContractId)
                .OnDelete(DeleteBehavior.Cascade); // o NoAction si prefieres controlar la eliminación manualmente
        }
    }
}
