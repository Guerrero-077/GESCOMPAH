using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Entity.Infrastructure.Context
{
    public sealed class PostgresDbContextFactory : IDesignTimeDbContextFactory<PostgresDbContext>
    {
        public PostgresDbContext CreateDbContext(string[] args)
        {
            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";

            var cfg = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true)
                .AddJsonFile($"appsettings.{env}.json", optional: true)
                .AddEnvironmentVariables()
                .Build();

            // Permitir override por CLI: --connection "..."
            var conn = GetArg(args, "--connection") ?? cfg.GetConnectionString("Postgres")
                      ?? throw new InvalidOperationException("Falta ConnectionStrings:Postgres");

            var opts = new DbContextOptionsBuilder<PostgresDbContext>()
                .UseNpgsql(conn, npg => npg.MigrationsAssembly(typeof(PostgresDbContext).Assembly.FullName))
                .Options;

            return new PostgresDbContext(opts);
        }

        private static string? GetArg(string[] args, string key)
        {
            var i = Array.IndexOf(args, key);
            return (i >= 0 && i + 1 < args.Length) ? args[i + 1] : null;
        }
    }
}
