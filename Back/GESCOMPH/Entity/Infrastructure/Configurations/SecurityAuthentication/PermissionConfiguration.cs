using Entity.Domain.Models.Implements.SecurityAuthentication;
using Entity.Infrastructure.Configurations.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Entity.Infrastructure.Configurations.SecurityAuthentication
{
    public class PermissionConfiguration : BaseModelGenericConfiguration<Permission>
    {
        public override void Configure(EntityTypeBuilder<Permission> builder)
        {
            base.Configure(builder);

            builder.ToTable("Permissions");

            builder.HasMany(p => p.RolFormPermissions)
                .WithOne(r => r.Permission)
                .HasForeignKey(r => r.PermissionId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
