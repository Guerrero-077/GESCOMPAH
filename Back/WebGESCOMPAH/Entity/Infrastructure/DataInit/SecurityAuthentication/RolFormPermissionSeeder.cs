using Entity.Domain.Models.Implements.SecurityAuthentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Entity.Infrastructure.DataInit.SecurityAuthentication
{
    public class RolFormPermissionSeeder : IEntityTypeConfiguration<RolFormPermission>
    {
        public void Configure(EntityTypeBuilder<RolFormPermission> builder)
        {
            var seedDate = DateTime.SpecifyKind(new DateTime(2025, 01, 01), DateTimeKind.Utc);

            int idCounter = 1;
            const int rolId = 1;
            int[] permissionIds = { 1, 2, 3, 4 };

            // Formularios del 1 al 10
            for (int formId = 1; formId <= 10; formId++)
            {
                foreach (var permissionId in permissionIds)
                {
                    builder.HasData(new RolFormPermission
                    {
                        Id = idCounter++,
                        RolId = rolId,
                        FormId = formId,
                        PermissionId = permissionId,
                        IsDeleted = false,
                        Active = true,
                        CreatedAt = seedDate
                    });
                }
            }
        }
    }
}
