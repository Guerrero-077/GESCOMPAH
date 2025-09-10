using Entity.Domain.Models.Implements.AdministrationSystem;
using Entity.Domain.Models.Implements.Business;
using Entity.Domain.Models.Implements.Persons;
using Entity.Domain.Models.Implements.SecurityAuthentication;
using Entity.Domain.Models.Implements.Utilities;
using Entity.DTOs.Implements.SecurityAuthentication.Auth.RestPasword;
using Entity.Infrastructure.Configurations.SecurityAuthentication;
using Microsoft.EntityFrameworkCore;

namespace Entity.Infrastructure.Context
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Images>().HasQueryFilter(img => !img.IsDeleted);
            //modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            modelBuilder.Entity<ObligationMonth>(e =>
            {
                e.Property(p => p.UvtQtyApplied).HasPrecision(18, 2);
                e.Property(p => p.UvtValueApplied).HasPrecision(18, 2);
                e.Property(p => p.VatRateApplied).HasPrecision(5, 4);  // tasas 0..1 con 4 decimales
                e.Property(p => p.BaseAmount).HasPrecision(18, 2);
                e.Property(p => p.VatAmount).HasPrecision(18, 2);
                e.Property(p => p.TotalAmount).HasPrecision(18, 2);
                e.Property(p => p.LateAmount).HasPrecision(18, 2);
            });

            // Idempotencia dura: evita duplicados por período
            modelBuilder.Entity<ObligationMonth>()
                .HasIndex(o => new { o.ContractId, o.Year, o.Month })
                .IsUnique();


            // Apply configurations from the assembly where UserConfiguration is defined
            //modelBuilder.ApplyConfigurationsFromAssembly(typeof(UserConfiguration).Assembly);


            /// <summary>
            /// Aplica automáticamente todas las configuraciones de entidades que implementan
            /// <c>IEntityTypeConfiguration&lt;TEntity&gt;</c> desde el ensamblado donde se encuentra <see cref="UserConfiguration" />.
            /// </summary>
            /// <remarks>
            /// Este método escanea el ensamblado especificado y registra todas las clases que implementan
            /// <see cref="Microsoft.EntityFrameworkCore.IEntityTypeConfiguration{TEntity}" />.
            /// 
            /// ✅ Beneficios:
            /// - Evita registrar cada configuración manualmente (como <c>modelBuilder.ApplyConfiguration(new XConfiguration())</c>).
            /// - Escalable: al agregar nuevas entidades/configuraciones, se aplican automáticamente.
            /// - Limpieza y mantenibilidad en <see cref="DbContext" 
            /// </remarks>
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(UserConfiguration).Assembly);


        }

        public DbSet<Person> Persons { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Rol> Roles { get; set; }
        public DbSet<RolUser> RolUsers { get; set; }
        public DbSet<PasswordResetCode> PasswordResetCodes { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<Form> Forms { get; set; }
        public DbSet<Domain.Models.Implements.AdministrationSystem.Module> Modules { get; set; }
        public DbSet<FormModule> FormModules { get; set; }
        public DbSet<RolFormPermission> RolFormPermissions { get; set; }

        //  Business
        public DbSet<Plaza> plaza { get; set; }
        public DbSet<Establishment> Establishment { get; set; }
        public DbSet<Images> Images { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; } = null!;

        public DbSet<SystemParameter> systemParameters { get; set; }

        public DbSet<Contract> contracts { get; set; }
        //public DbSet<ContractTerms> ContractTerms { get; set; }
        public DbSet<PremisesLeased> premisesLeaseds { get; set; }
        public DbSet<Clause> clauses { get; set; }
        public DbSet<ContractClause> contractClauses { get; set; }
        public DbSet<ObligationMonth> obligationMonths { get; set; }
    }
}
