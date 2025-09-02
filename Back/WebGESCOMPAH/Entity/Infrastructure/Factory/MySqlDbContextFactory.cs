using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Entity.Infrastructure.Context
{
    /// <summary>
    /// Fábrica de diseño para MySqlApplicationDbContext (migraciones, scaffolding).
    /// Permite usar: dotnet ef migrations add Init -c MySqlApplicationDbContext
    /// </summary>
    public sealed class MySqlDbContextFactory : IDesignTimeDbContextFactory<MySqlApplicationDbContext>
    {
        public MySqlApplicationDbContext CreateDbContext(string[] args)
        {
            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";

            var cfg = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env}.json", optional: true)
                .AddEnvironmentVariables()
                .Build();

            // Permite sobrescribir la cadena desde args: --connection
            var conn = GetArg(args, "--connection")
                       ?? cfg.GetConnectionString("MySql")
                       ?? throw new InvalidOperationException("Falta ConnectionStrings:MySql en appsettings");

            var opts = new DbContextOptionsBuilder<MySqlApplicationDbContext>()
                .UseMySql(conn, ServerVersion.AutoDetect(conn), mySql =>
                {
                    mySql.MigrationsAssembly(typeof(MySqlApplicationDbContext).Assembly.FullName);
                    mySql.EnableStringComparisonTranslations(); // LIKE/StartsWith/EndsWith natural
                })
                .Options;

            return new MySqlApplicationDbContext(opts);
        }

        private static string? GetArg(string[] args, string key)
        {
            var i = Array.IndexOf(args, key);
            return (i >= 0 && i + 1 < args.Length) ? args[i + 1] : null;
        }
    }
}
