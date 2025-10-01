using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.Reflection;

namespace Entity.Infrastructure.Context
{
    /// <summary>
    /// Fábrica de diseño para MySqlApplicationDbContext (migraciones, scaffolding).
    /// dotnet ef migrations add Init -c MySqlApplicationDbContext
    /// </summary>
    public sealed class MySqlDbContextFactory : IDesignTimeDbContextFactory<MySqlApplicationDbContext>
    {
        public MySqlApplicationDbContext CreateDbContext(string[] args)
        {
            // Subir desde bin/... hasta el directorio que contiene el .csproj
            var assemblyDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;
            var projectDir = AscendToProjectRoot(assemblyDir);

            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";

            var cfg = new ConfigurationBuilder()
                .SetBasePath(projectDir)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
                .AddJsonFile($"appsettings.{env}.json", optional: true)
                .AddEnvironmentVariables()
                .Build();

            // Permite override con --connection
            var conn = GetArg(args, "--connection")
                       ?? cfg.GetConnectionString("MySql")
                       ?? throw new InvalidOperationException("Falta ConnectionStrings:MySql en appsettings");

            // *** Fijar versión para NO conectar en design-time ***
            var serverVersion = new MySqlServerVersion(new Version(8, 0, 36));
            // Si usas MariaDB, cambia a: new MariaDbServerVersion(new Version(10, 11, 0));

            var options = new DbContextOptionsBuilder<MySqlApplicationDbContext>()
                .UseMySql(conn, serverVersion, mySql =>
                {
                    mySql.MigrationsAssembly(typeof(MySqlApplicationDbContext).Assembly.FullName);
                    mySql.EnableStringComparisonTranslations();
                    mySql.MigrationsHistoryTable("__EFMigrationsHistory", "infra");
                })
                .Options;

            return new MySqlApplicationDbContext(options);
        }

        private static string? GetArg(string[] args, string key)
        {
            var i = Array.IndexOf(args, key);
            return (i >= 0 && i + 1 < args.Length) ? args[i + 1] : null;
        }

        private static string AscendToProjectRoot(string start)
        {
            var dir = new DirectoryInfo(start);
            while (dir != null)
            {
                if (dir.GetFiles("*.csproj").Length > 0) return dir.FullName;
                dir = dir.Parent!;
            }
            return start;
        }
    }
}