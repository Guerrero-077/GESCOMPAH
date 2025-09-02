using Entity.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<DbContext>
{
    public DbContext CreateDbContext(string[] args)
    {
        var providerIndex = Array.IndexOf(args, "--provider");
        var provider = providerIndex >= 0 && args.Length > providerIndex + 1
            ? args[providerIndex + 1]
            : "SqlServer";

        var config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();

        var conn = config.GetConnectionString(provider);

        switch (provider)
        {
            case "Postgres":
                {
                    var builder = new DbContextOptionsBuilder<PostgresDbContext>();
                    builder.UseNpgsql(conn);
                    return new PostgresDbContext(builder.Options);
                }
            default:
                {
                    var builder = new DbContextOptionsBuilder<ApplicationDbContext>();
                    builder.UseSqlServer(conn);
                    return new ApplicationDbContext(builder.Options);
                }
        }
    }
}
