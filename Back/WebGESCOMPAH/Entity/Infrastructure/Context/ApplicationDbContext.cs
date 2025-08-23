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
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<User>()
             .HasOne(u => u.Person)
             .WithOne(p => p.User)
             .HasForeignKey<User>(u => u.PersonId)
             .OnDelete(DeleteBehavior.Cascade); // o Restrict si no quieres borrado en cascada

            modelBuilder.Entity<Images>(e =>
            {
                e.HasOne(i => i.Establishment)
                 .WithMany(e => e.Images)
                 .HasForeignKey(i => i.EstablishmentId)
                 .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<RefreshToken>(ent =>
            {
                ent.HasKey(e => e.Id);
                ent.HasIndex(e => e.TokenHash).IsUnique(false);
                ent.HasIndex(e => e.UserId);
                ent.Property(e => e.TokenHash).IsRequired().HasMaxLength(128);
            });

            //Data init
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Images>().HasQueryFilter(img => !img.IsDeleted);
            //modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());


            // Apply configurations from the assembly where UserConfiguration is defined
            //modelBuilder.ApplyConfigurationsFromAssembly(typeof(UserConfiguration).Assembly);
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
    }
}
