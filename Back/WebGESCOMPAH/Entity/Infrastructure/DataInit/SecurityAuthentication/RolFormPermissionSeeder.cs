using Entity.Domain.Models.Implements.SecurityAuthentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Entity.Infrastructure.DataInit.SecurityAuthentication
{
    public class RolFormPermissionSeeder : IEntityTypeConfiguration<RolFormPermission>
    {
        public void Configure(EntityTypeBuilder<RolFormPermission> builder)
        {
            // Opcional: UTC
            var seedDate = DateTime.SpecifyKind(new DateTime(2025, 01, 01), DateTimeKind.Utc);

            builder.HasData(
                // Form 1
                new RolFormPermission { Id = 1, RolId = 1, FormId = 1, PermissionId = 1, IsDeleted = false, Active = true, CreatedAt = seedDate },
                new RolFormPermission { Id = 2, RolId = 1, FormId = 1, PermissionId = 2, IsDeleted = false, Active = true, CreatedAt = seedDate },
                new RolFormPermission { Id = 3, RolId = 1, FormId = 1, PermissionId = 3, IsDeleted = false, Active = true, CreatedAt = seedDate },
                new RolFormPermission { Id = 4, RolId = 1, FormId = 1, PermissionId = 4, IsDeleted = false, Active = true, CreatedAt = seedDate },

                // Form 2
                new RolFormPermission { Id = 5, RolId = 1, FormId = 2, PermissionId = 1, IsDeleted = false, Active = true, CreatedAt = seedDate },
                new RolFormPermission { Id = 6, RolId = 1, FormId = 2, PermissionId = 2, IsDeleted = false, Active = true, CreatedAt = seedDate },
                new RolFormPermission { Id = 7, RolId = 1, FormId = 2, PermissionId = 3, IsDeleted = false, Active = true, CreatedAt = seedDate },
                new RolFormPermission { Id = 8, RolId = 1, FormId = 2, PermissionId = 4, IsDeleted = false, Active = true, CreatedAt = seedDate },

                // Form 3
                new RolFormPermission { Id = 9, RolId = 1, FormId = 3, PermissionId = 1, IsDeleted = false, Active = true, CreatedAt = seedDate },
                new RolFormPermission { Id = 10, RolId = 1, FormId = 3, PermissionId = 2, IsDeleted = false, Active = true, CreatedAt = seedDate },
                new RolFormPermission { Id = 11, RolId = 1, FormId = 3, PermissionId = 3, IsDeleted = false, Active = true, CreatedAt = seedDate },
                new RolFormPermission { Id = 12, RolId = 1, FormId = 3, PermissionId = 4, IsDeleted = false, Active = true, CreatedAt = seedDate },

                // Form 4
                new RolFormPermission { Id = 13, RolId = 1, FormId = 4, PermissionId = 1, IsDeleted = false, Active = true, CreatedAt = seedDate },
                new RolFormPermission { Id = 14, RolId = 1, FormId = 4, PermissionId = 2, IsDeleted = false, Active = true, CreatedAt = seedDate },
                new RolFormPermission { Id = 15, RolId = 1, FormId = 4, PermissionId = 3, IsDeleted = false, Active = true, CreatedAt = seedDate },
                new RolFormPermission { Id = 16, RolId = 1, FormId = 4, PermissionId = 4, IsDeleted = false, Active = true, CreatedAt = seedDate },

                // Form 5
                new RolFormPermission { Id = 17, RolId = 1, FormId = 5, PermissionId = 1, IsDeleted = false, Active = true, CreatedAt = seedDate },
                new RolFormPermission { Id = 18, RolId = 1, FormId = 5, PermissionId = 2, IsDeleted = false, Active = true, CreatedAt = seedDate },
                new RolFormPermission { Id = 19, RolId = 1, FormId = 5, PermissionId = 3, IsDeleted = false, Active = true, CreatedAt = seedDate },
                new RolFormPermission { Id = 20, RolId = 1, FormId = 5, PermissionId = 4, IsDeleted = false, Active = true, CreatedAt = seedDate },

                // >>> Formularios faltantes <<<

                // Form 6
                new RolFormPermission { Id = 21, RolId = 1, FormId = 6, PermissionId = 1, IsDeleted = false, Active = true, CreatedAt = seedDate },
                new RolFormPermission { Id = 22, RolId = 1, FormId = 6, PermissionId = 2, IsDeleted = false, Active = true, CreatedAt = seedDate },
                new RolFormPermission { Id = 23, RolId = 1, FormId = 6, PermissionId = 3, IsDeleted = false, Active = true, CreatedAt = seedDate },
                new RolFormPermission { Id = 24, RolId = 1, FormId = 6, PermissionId = 4, IsDeleted = false, Active = true, CreatedAt = seedDate },

                // Form 7
                new RolFormPermission { Id = 25, RolId = 1, FormId = 7, PermissionId = 1, IsDeleted = false, Active = true, CreatedAt = seedDate },
                new RolFormPermission { Id = 26, RolId = 1, FormId = 7, PermissionId = 2, IsDeleted = false, Active = true, CreatedAt = seedDate },
                new RolFormPermission { Id = 27, RolId = 1, FormId = 7, PermissionId = 3, IsDeleted = false, Active = true, CreatedAt = seedDate },
                new RolFormPermission { Id = 28, RolId = 1, FormId = 7, PermissionId = 4, IsDeleted = false, Active = true, CreatedAt = seedDate },

                // Form 8
                new RolFormPermission { Id = 29, RolId = 1, FormId = 8, PermissionId = 1, IsDeleted = false, Active = true, CreatedAt = seedDate },
                new RolFormPermission { Id = 30, RolId = 1, FormId = 8, PermissionId = 2, IsDeleted = false, Active = true, CreatedAt = seedDate },
                new RolFormPermission { Id = 31, RolId = 1, FormId = 8, PermissionId = 3, IsDeleted = false, Active = true, CreatedAt = seedDate },
                new RolFormPermission { Id = 32, RolId = 1, FormId = 8, PermissionId = 4, IsDeleted = false, Active = true, CreatedAt = seedDate },

                // Form 9
                new RolFormPermission { Id = 33, RolId = 1, FormId = 9, PermissionId = 1, IsDeleted = false, Active = true, CreatedAt = seedDate },
                new RolFormPermission { Id = 34, RolId = 1, FormId = 9, PermissionId = 2, IsDeleted = false, Active = true, CreatedAt = seedDate },
                new RolFormPermission { Id = 35, RolId = 1, FormId = 9, PermissionId = 3, IsDeleted = false, Active = true, CreatedAt = seedDate },
                new RolFormPermission { Id = 36, RolId = 1, FormId = 9, PermissionId = 4, IsDeleted = false, Active = true, CreatedAt = seedDate }
            );
        }
    }
}
