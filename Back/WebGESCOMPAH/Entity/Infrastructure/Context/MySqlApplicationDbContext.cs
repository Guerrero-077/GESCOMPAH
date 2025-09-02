using System.Reflection;
using Entity.Domain.Models.Implements.AdministrationSystem;
using Entity.Domain.Models.Implements.Business;
using Entity.Domain.Models.Implements.Persons;
using Entity.Domain.Models.Implements.SecurityAuthentication;
using Entity.Domain.Models.Implements.Utilities;
using Entity.Domain.Models.ModelBase; // <-- BaseModel
using Entity.DTOs.Implements.SecurityAuthentication.Auth.RestPasword;
using Entity.Infrastructure.Configurations.SecurityAuthentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion; // <-- ValueConverter

namespace Entity.Infrastructure.Context
{
    /// <summary>
    /// DbContext exclusivo para MySQL 8+ con Pomelo.EntityFrameworkCore.MySql.
    /// Mantiene los mismos DbSet que ApplicationDbContext y aplica convenciones para MySQL.
    /// </summary>
    public class MySqlApplicationDbContext : DbContext
    {
        public MySqlApplicationDbContext(DbContextOptions<MySqlApplicationDbContext> options)
            : base(options) { }

        // Convenciones globales solo para DateTime
        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            configurationBuilder.Properties<DateTime>().HaveColumnType("datetime(6)");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Charset/Collation recomendados para MySQL 8
            modelBuilder
                .HasCharSet("utf8mb4")
                .UseCollation("utf8mb4_0900_ai_ci");

            // Filtro soft-delete en Images
            modelBuilder.Entity<Images>().HasQueryFilter(img => !img.IsDeleted);

            // ========= Ajuste clave para BaseModel.CreatedAt (DateTime) =========
            // Guardamos en UTC como datetime(6) y default en BD con CURRENT_TIMESTAMP(6)
            var dtoToUtc = new ValueConverter<DateTime, DateTime>(
                v => v.ToUniversalTime(),
                v => DateTime.SpecifyKind(v, DateTimeKind.Utc));

            foreach (var et in modelBuilder.Model.GetEntityTypes())
            {
                if (typeof(BaseModel).IsAssignableFrom(et.ClrType))
                {
                    var b = modelBuilder.Entity(et.ClrType);

                    b.Property(nameof(BaseModel.CreatedAt))
                     .HasConversion(dtoToUtc)
                     .HasColumnType("datetime(6)")
                     .IsRequired()
                     .ValueGeneratedOnAdd()
                     .HasDefaultValueSql("CURRENT_TIMESTAMP(6)");
                }
            }
            // =========================================================================

            // Aplica todas las configuraciones de tu ensamblado
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(UserConfiguration).Assembly);
        }

        // ===== DbSets (idénticos a tu ApplicationDbContext) =====
        public DbSet<Person> Persons { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Rol> Roles { get; set; }
        public DbSet<RolUser> RolUsers { get; set; }
        public DbSet<PasswordResetCode> PasswordResetCodes { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<Form> Forms { get; set; }
        public DbSet<Entity.Domain.Models.Implements.AdministrationSystem.Module> Modules { get; set; }
        public DbSet<FormModule> FormModules { get; set; }
        public DbSet<RolFormPermission> RolFormPermissions { get; set; }

        // Business
        public DbSet<Plaza> plaza { get; set; }
        public DbSet<Establishment> Establishment { get; set; }
        public DbSet<Images> Images { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; } = null!;
        public DbSet<SystemParameter> systemParameters { get; set; }
        public DbSet<Contract> contracts { get; set; }
        public DbSet<PremisesLeased> premisesLeaseds { get; set; }
        public DbSet<Clause> clauses { get; set; }
        public DbSet<ContractClause> contractClauses { get; set; }
    }
}
