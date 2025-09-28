using Entity.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace WebGESCOMPAH.Extensions
{
    public static class MigrationManager
    {
        // Aplica migraciones según "MigrateOnStartupTargets": ["SqlServer", "Postgres", "MySql"]
        public static void MigrateAllDatabases(IServiceProvider services, IConfiguration config)
        {
            var targets = config.GetSection("MigrateOnStartupTargets").Get<string[]>() ?? Array.Empty<string>();

            using var scope = services.CreateScope();
            var sp = scope.ServiceProvider;
            var log = sp.GetRequiredService<ILoggerFactory>().CreateLogger("MigrationManager");

            foreach (var raw in targets)
            {
                var t = raw?.Trim()?.ToLowerInvariant();
                if (string.IsNullOrWhiteSpace(t)) continue;

                try
                {
                    switch (t)
                    {
                        case "sqlserver":
                            {
                                var ctx = sp.GetService<ApplicationDbContext>();
                                if (ctx == null) { log.LogWarning("SQL Server no configurado: se omite."); break; }
                                log.LogInformation("➡️ Migrando SQL Server...");
                                ctx.Database.Migrate();
                                log.LogInformation("✅ Migración completada para SqlServer");
                                break;
                            }
                        case "postgres":
                            {
                                var ctx = sp.GetService<PostgresDbContext>();
                                if (ctx == null) { log.LogWarning("PostgreSQL no configurado: se omite."); break; }
                                log.LogInformation("➡️ Migrando PostgreSQL...");
                                ctx.Database.Migrate();
                                log.LogInformation("✅ Migración completada para Postgres");
                                break;
                            }
                        case "mysql":
                            {
                                var ctx = sp.GetService<MySqlApplicationDbContext>();
                                if (ctx == null) { log.LogWarning("MySQL no configurado: se omite."); break; }
                                log.LogInformation("➡️ Migrando MySQL...");
                                ctx.Database.Migrate();
                                log.LogInformation("✅ Migración completada para MySql");
                                break;
                            }
                        default:
                            log.LogWarning("Objetivo de migración desconocido: {Target}", raw);
                            break;
                    }
                }
                catch (Exception ex)
                {
                    log.LogError(ex, "❌ Error aplicando migraciones para {Target}", raw);
                    throw; // falla rápido si una migración no aplica
                }
            }
        }
    }
}
