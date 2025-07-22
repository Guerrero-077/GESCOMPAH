using Entity.Domain.Models.Implements.SecurityAuthentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Entity.Infrastructure.DataInit.SecurityAuthentication
{
    public class RolFormPermissionSeeder : IEntityTypeConfiguration<RolFormPermission>
    {
        public void Configure(EntityTypeBuilder<RolFormPermission> builder)
        {
            var seedDate = new DateTime(2025, 01, 01);

            var id = 1;
            var seed = new List<RolFormPermission>();

            for (int formId = 1; formId <= 3; formId++) // 3 formularios
            {
                for (int permissionId = 1; permissionId <= 4; permissionId++) // 4 permisos
                {
                    seed.Add(new RolFormPermission
                    {
                        Id = id++,
                        RolId = 1, // Admin
                        FormId = formId,
                        PermissionId = permissionId,
                         IsDeleted = false,
                        Active = true,
                        CreatedAt = seedDate
                    });
                }
            }

            builder.HasData(seed);
        }
    }
}