using Hangfire;
using Hangfire.SqlServer;
using TimeZoneConverter;
using WebGESCOMPAH.RealTime;
using WebGESCOMPAH.Security;

namespace WebGESCOMPAH.Extensions.Infrastructure
{
    /// <summary>
    /// Registro y configuración de Hangfire (storage, servidor, dashboard y jobs).
    /// </summary>
    /// <remarks>
    /// Qué hace: configura almacenamiento en SQL Server con opciones recomendadas, arranca el
    /// servidor de trabajo y expone el Dashboard; además registra/actualiza los trabajos recurrentes
    /// de obligaciones mensuales y expiración de contratos.
    /// 
    /// Por qué: orquestar tareas en background de manera resiliente y observable.
    /// 
    /// Para qué: separar la infraestructura de jobs del resto del arranque.
    /// </remarks>
    public static class HangfireExtensions
    {
        /// <summary>
        /// Registra Hangfire (storage + servidor) con SQL Server.
        /// </summary>
        public static IServiceCollection AddHangfireServices(this IServiceCollection services, IConfiguration configuration)
        {
            var hfConn = configuration.GetConnectionString("SqlServer")
                       ?? throw new InvalidOperationException("Falta ConnectionStrings:SqlServer en appsettings.json");

            services.AddHangfire(cfg =>
            {
                cfg.SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
                   .UseSimpleAssemblyNameTypeSerializer()
                   .UseRecommendedSerializerSettings()
                   .UseSqlServerStorage(
                       hfConn,
                       new SqlServerStorageOptions
                       {
                           SchemaName = configuration["Hangfire:Schema"] ?? "hangfire",
                           UseRecommendedIsolationLevel = true,
                           TryAutoDetectSchemaDependentOptions = true,
                           SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                           QueuePollInterval = TimeSpan.Zero
                       });
            });

            services.AddHangfireServer(options =>
            {
                options.Queues = new[] { "default", "maintenance" };
            });

            return services;
        }

        /// <summary>
        /// Habilita el Dashboard y programa los trabajos recurrentes.
        /// </summary>
        public static IApplicationBuilder UseHangfireDashboardAndJobs(this IApplicationBuilder app, IConfiguration configuration)
        {
            var env = (app as WebApplication)!.Environment;
            var dashboardAuth = env.IsDevelopment()
                ? (Hangfire.Dashboard.IDashboardAuthorizationFilter)new HangfireDashboardAuth()
                : new HangfireDashboardAuth();

            app.UseHangfireDashboard("/hangfire", new DashboardOptions
            {
                Authorization = new[] { dashboardAuth }
            });

            var tz = TZConvert.GetTimeZoneInfo(configuration["Hangfire:TimeZoneIana"] ?? "America/Bogota");

            var cronObligations = configuration["Hangfire:CronObligations"] ?? "15 2 1 * *";
            RecurringJob.AddOrUpdate<ObligationJobs>(
                "obligations-monthly",
                j => j.GenerateForCurrentMonthAsync(JobCancellationToken.Null),
                cronObligations,
                new RecurringJobOptions { TimeZone = tz, QueueName = "maintenance" }
            );

            if (configuration.GetValue<bool>("Contracts:Expiration:Enabled"))
            {
                var cronContracts = configuration["Contracts:Expiration:Cron"] ?? "*/10 * * * *";
                RecurringJob.AddOrUpdate<ContractJobs>(
                    "contracts-expiration",
                    j => j.RunExpirationSweepAsync(CancellationToken.None),
                    cronContracts,
                    new RecurringJobOptions { TimeZone = tz, QueueName = "default" }
                );
            }

            return app;
        }
    }
}

