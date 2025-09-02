using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Entity.Infrastructure.Context
{
    public sealed class SqlServerDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";

            var cfg = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true)
                .AddJsonFile($"appsettings.{env}.json", optional: true)
                .AddEnvironmentVariables()
                .Build();

            var conn = GetArg(args, "--connection") ?? cfg.GetConnectionString("SqlServer")
                      ?? throw new InvalidOperationException("Falta ConnectionStrings:SqlServer");

            var opts = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlServer(conn, sql => sql.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName))
                .Options;

            return new ApplicationDbContext(opts);
        }

        private static string? GetArg(string[] args, string key)
        {
            var i = Array.IndexOf(args, key);
            return (i >= 0 && i + 1 < args.Length) ? args[i + 1] : null;
        }
    }
}
