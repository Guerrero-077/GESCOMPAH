using Entity.Domain.Models.Implements.AdministrationSystem;
using Entity.Infrastructure.Configurations.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Entity.Infrastructure.Configurations.AdministrationSystem
{
    public class FormConfiguration : BaseModelGenericConfiguration<Form>
    {
        public override void Configure(EntityTypeBuilder<Form> builder)
        {
            base.Configure(builder);
            builder.ToTable("Forms");
            builder.Property(f => f.Route)
                .IsRequired()
                .HasMaxLength(200);
            builder.HasMany(f => f.RolFormPermissions)
                .WithOne(rfp => rfp.Form)
                .HasForeignKey(rfp => rfp.FormId)
                .OnDelete(DeleteBehavior.Restrict);
            builder.HasMany(f => f.FormModules)
                .WithOne(fm => fm.Form)
                .HasForeignKey(fm => fm.FormId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
