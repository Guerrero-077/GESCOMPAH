using Entity.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace WebGESCOMPAH.Extensions
{
    public static class DatabaseService
    {
        //public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
        //{
        //    services.AddDbContext<ApplicationDbContext>(opciones => opciones
        //      .UseSqlServer("name=DefaultConnection"));

        //    return services;
        //}

        public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration config)
        {
            var sql = config.GetConnectionString("SqlServer");
            var pg = config.GetConnectionString("Postgres");
            var my = config.GetConnectionString("MySql");

            if (!string.IsNullOrWhiteSpace(sql))
            {
                services.AddDbContext<ApplicationDbContext>(opt =>
                    opt.UseSqlServer(sql, s =>
                    {
                        s.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName);
                        s.EnableRetryOnFailure();
                        s.CommandTimeout(60);
                    }));
            }

            if (!string.IsNullOrWhiteSpace(pg))
            {
                services.AddDbContext<PostgresDbContext>(opt =>
                    opt.UseNpgsql(pg, n =>
                    {
                        n.MigrationsAssembly(typeof(PostgresDbContext).Assembly.FullName);
                        n.EnableRetryOnFailure();
                        n.CommandTimeout(60);
                    }));
            }

            if (!string.IsNullOrWhiteSpace(my))
            {
                services.AddDbContext<MySqlApplicationDbContext>(opt =>
                    opt.UseMySql(my, ServerVersion.AutoDetect(my), m =>
                    {
                        m.MigrationsAssembly(typeof(MySqlApplicationDbContext).Assembly.FullName);
                        m.EnableStringComparisonTranslations(); // para Contains, StartsWith, EndsWith
                    })
                    .EnableDetailedErrors()
                    .EnableSensitiveDataLogging() // ⚠️ solo en desarrollo
                );
            }

            return services;
        }
    }

}