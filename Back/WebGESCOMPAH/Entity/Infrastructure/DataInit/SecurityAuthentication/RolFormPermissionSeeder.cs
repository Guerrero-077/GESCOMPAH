using Entity.Domain.Models.Implements.SecurityAuthentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Entity.Infrastructure.DataInit.SecurityAuthentication
{
    public class RolFormPermissionSeeder : IEntityTypeConfiguration<RolFormPermission>
    {
        /// <summary>
        /// Permisos por rol:
        ///  - Rol 1 (Administrador): permisos {1..4} en formularios {1..11}.
        ///  - Rol 2 (Arrendatario): permiso {1=Read} en formularios {1,2,4,11}.
        /// Notas:
        ///  - PermissionId: 1=Read, 2=Create, 3=Update, 4=Delete (ajústalo a tu catálogo real).
        ///  - Usa PK fija en HasData; EF habilita IDENTITY_INSERT en migraciones.
        ///  - Índice único evita duplicados (RolId, FormId, PermissionId).
        /// </summary>
        public void Configure(EntityTypeBuilder<RolFormPermission> builder)
        {
            var seedDate = DateTime.SpecifyKind(new DateTime(2025, 01, 01), DateTimeKind.Utc);

            // Unicidad lógica
            builder.HasIndex(x => new { x.RolId, x.FormId, x.PermissionId }).IsUnique();

            // Catálogo de permisos (confirma que coincidan con tu tabla Permission)
            int READ = 1, CREATE = 2, UPDATE = 3, DELETE = 4;

            // Formularios (según tu FormSeeder)
            const int FORMS_MIN = 1;
            const int FORMS_MAX = 11; // ¡Ojo! Incluye Configuración Principal (Id=11)

            // Conjunto de forms permitidos al Arrendatario (solo Read):
            int[] tenantForms = { 1, 2, 4, 11 };

            var seeds = new List<RolFormPermission>();
            var id = 1; // contador determinista; si prefieres, arranca en 1001 para reducir colisiones

            // --- Rol 1: Administrador (full control en todos los forms) ---
            for (int formId = FORMS_MIN; formId <= FORMS_MAX; formId++)
            {
                seeds.Add(new RolFormPermission { Id = id++, RolId = 1, FormId = formId, PermissionId = READ, IsDeleted = false, Active = true, CreatedAt = seedDate });
                seeds.Add(new RolFormPermission { Id = id++, RolId = 1, FormId = formId, PermissionId = CREATE, IsDeleted = false, Active = true, CreatedAt = seedDate });
                seeds.Add(new RolFormPermission { Id = id++, RolId = 1, FormId = formId, PermissionId = UPDATE, IsDeleted = false, Active = true, CreatedAt = seedDate });
                seeds.Add(new RolFormPermission { Id = id++, RolId = 1, FormId = formId, PermissionId = DELETE, IsDeleted = false, Active = true, CreatedAt = seedDate });
            }

            // --- Rol 2: Arrendatario (solo Read en Inicio, Establecimientos, Contratos, Configuración) ---
            foreach (var formId in tenantForms.Distinct().OrderBy(x => x))
            {
                seeds.Add(new RolFormPermission
                {
                    Id = id++,
                    RolId = 2,
                    FormId = formId,
                    PermissionId = READ,
                    IsDeleted = false,
                    Active = true,
                    CreatedAt = seedDate
                });
            }

            builder.HasData(seeds);
        }
    }
}
