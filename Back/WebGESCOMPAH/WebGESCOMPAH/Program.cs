using Business.Interfaces.PDF;
using Business.Services.Utilities.PDF;
using CloudinaryDotNet;
using Entity.Domain.Models.Implements.SecurityAuthentication;
using Entity.DTOs.Validations.SecurityAuthentication.Auth;
using FluentValidation;
using FluentValidation.AspNetCore;
using Hangfire;
using Hangfire.SqlServer;
using Hangfire.Server;                   // ✅ Necesario para JobCancellationToken.Null
using TimeZoneConverter;
using Utilities.Helpers.CloudinaryHelper;
using WebGESCOMPAH.Extensions;
using WebGESCOMPAH.RealTime;
using WebGESCOMPAH.Security;

var builder = WebApplication.CreateBuilder(args);

// --------------------------
// CONFIGURACIÓN Y SERVICIOS
// --------------------------

// PDF
builder.Services.AddScoped<IContractPdfGeneratorService, ContractPdfService>();

// Controllers / Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// CORS, JWT, DB, Servicios personalizados (extensiones propias)
builder.Services.AddCustomCors(builder.Configuration);
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));
builder.Services.Configure<CookieSettings>(builder.Configuration.GetSection("Cookie"));
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddApplicationServices();   // ← aquí registras tus servicios de capa Business/Data

// MULTI-DB (SqlServer + Postgres según tu extensión)
builder.Services.AddDatabase(builder.Configuration);

// Validaciones
builder.Services.AddValidatorsFromAssemblyContaining<RegisterDtoValidator>();
builder.Services.AddFluentValidationAutoValidation();

// SignalR
builder.Services.AddSignalR();

// Cloudinary
var cloudinaryConfig = builder.Configuration.GetSection("Cloudinary");
var cloudinary = new Cloudinary(new Account(
    cloudinaryConfig["CloudName"],
    cloudinaryConfig["ApiKey"],
    cloudinaryConfig["ApiSecret"]
));
cloudinary.Api.Secure = true;
builder.Services.AddSingleton(cloudinary);
builder.Services.AddScoped<CloudinaryUtility>();

// Jobs (orquestador Hangfire)
builder.Services.AddScoped<ObligationJobs>();

// --------------------------
// HANGFIRE (Storage en SQL Server)
// --------------------------
var hfConn = builder.Configuration.GetConnectionString("SqlServer")
           ?? throw new InvalidOperationException("Falta ConnectionStrings:SqlServer en appsettings.json");

builder.Services.AddHangfire(cfg =>
{
    cfg.SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
       .UseSimpleAssemblyNameTypeSerializer()
       .UseRecommendedSerializerSettings()
       .UseSqlServerStorage(
           hfConn,
           new SqlServerStorageOptions
           {
               SchemaName = builder.Configuration["Hangfire:Schema"] ?? "hangfire",
               UseRecommendedIsolationLevel = true,
               TryAutoDetectSchemaDependentOptions = true,
               SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
               QueuePollInterval = TimeSpan.Zero // long-polling
           });
});

// Servidor de procesos Hangfire — UNA sola vez
builder.Services.AddHangfireServer(options =>
{
    options.Queues = new[] { "default", "maintenance" };
});

var app = builder.Build();

// --------------------------
// MIDDLEWARE GLOBAL
// --------------------------
app.UseMiddleware<ExceptionMiddleware>();

app.UseStaticFiles();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "GESCOMPH API v1");
    c.RoutePrefix = "swagger";
});

app.UseCors();
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Migraciones multi-DB en arranque (tu helper)
MigrationManager.MigrateAllDatabases(app.Services, builder.Configuration);

// SignalR hubs
app.MapHub<ContractsHub>("/api/hubs/contracts");

// --------------------------
// HANGFIRE Dashboard + Recurring Job
// --------------------------

// En Dev: abre para loopback (HangfireDashboardAuth). En Prod: exige rol "Administrador".
var dashboardAuth = app.Environment.IsDevelopment()
    ? (Hangfire.Dashboard.IDashboardAuthorizationFilter)new HangfireDashboardAuth()
    : new HangfireDashboardAuth();

app.UseHangfireDashboard("/hangfire", new DashboardOptions
{
    Authorization = new[] { dashboardAuth }
});

// Cron y zona horaria desde appsettings (con fallback)
var cron = builder.Configuration["Hangfire:CronObligations"] ?? "15 2 1 * *"; // 1° de cada mes 02:15
var tz = TZConvert.GetTimeZoneInfo(builder.Configuration["Hangfire:TimeZoneIana"] ?? "America/Bogota");

// Registrar job recurrente (cola "maintenance")
RecurringJob.AddOrUpdate<ObligationJobs>(
    recurringJobId: "obligations-monthly",
    methodCall: j => j.GenerateForCurrentMonthAsync(JobCancellationToken.Null), 
    cronExpression: cron,
    options: new RecurringJobOptions { TimeZone = tz, QueueName = "maintenance" }
);

// (Opcional) Smoke test:
// BackgroundJob.Enqueue(() => Console.WriteLine($"Hangfire OK {DateTime.UtcNow:O}"));

// --------------------------
// RUN
// --------------------------
app.Run();
