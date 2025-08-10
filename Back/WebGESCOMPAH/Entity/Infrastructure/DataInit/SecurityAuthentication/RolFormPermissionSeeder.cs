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
                new RolFormPermission { Id = 20, RolId = 1, FormId = 5, PermissionId = 4, IsDeleted = false, Active = true, CreatedAt = seedDate }
            );
        }
    }
}
