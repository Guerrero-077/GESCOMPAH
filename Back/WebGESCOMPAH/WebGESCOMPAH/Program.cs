using System.Text.Json.Serialization;
using Business.Interfaces.PDF;
using Business.Services.Utilities.PDF;
using CloudinaryDotNet;
using Entity.Domain.Models.Implements.SecurityAuthentication;
using Entity.DTOs.Validations.SecurityAuthentication.Auth;
using FluentValidation;
using FluentValidation.AspNetCore;
using Hangfire;
using Hangfire.SqlServer;
using TimeZoneConverter;
using Utilities.Helpers.CloudinaryHelper;
using WebGESCOMPAH.Extensions;
using WebGESCOMPAH.RealTime;
using WebGESCOMPAH.Security;
using WebGESCOMPAH.Middleware;
using Entity.Infrastructure.Binder; // <-- para el FlexibleDecimalModelBinder

var builder = WebApplication.CreateBuilder(args);

// --------------------------
// CONFIGURACIÓN Y SERVICIOS
// --------------------------

// PDF
builder.Services.AddScoped<IContractPdfGeneratorService, ContractPdfService>();

// Controllers / Swagger
builder.Services
    .AddControllers(options =>
    {
        options.ModelBinderProviders.Insert(0, new FlexibleDecimalModelBinderProvider());
    })
    .AddJsonOptions(o =>
    {
        // permitir números como string en JSON
        o.JsonSerializerOptions.NumberHandling = JsonNumberHandling.AllowReadingFromString;
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// CORS, JWT, DB, Servicios personalizados (extensiones propias)
builder.Services.AddCustomCors(builder.Configuration);
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));
builder.Services.Configure<CookieSettings>(builder.Configuration.GetSection("Cookie"));
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddApplicationServices();   // registra Business/Data, incl. NotificationService

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
builder.Services.AddScoped<ContractJobs>();  // job para contratos

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

// Migraciones multi-DB en arranque
MigrationManager.MigrateAllDatabases(app.Services, builder.Configuration);

// SignalR hubs
app.MapHub<ContractsHub>("/api/hubs/contracts");

// Warmup Playwright/Chromium (evitar arranque en frío)
try
{
    using var scope = app.Services.CreateScope();
    var pdfGen = scope.ServiceProvider.GetRequiredService<IContractPdfGeneratorService>();
    await pdfGen.WarmupAsync();
}
catch (Exception ex)
{
    Console.WriteLine($"[Warmup PDF] Aviso: {ex.Message}");
}

// --------------------------
// HANGFIRE Dashboard + Recurring Jobs
// --------------------------
var dashboardAuth = app.Environment.IsDevelopment()
    ? (Hangfire.Dashboard.IDashboardAuthorizationFilter)new HangfireDashboardAuth()
    : new HangfireDashboardAuth();

app.UseHangfireDashboard("/hangfire", new DashboardOptions
{
    Authorization = new[] { dashboardAuth }
});

var tz = TZConvert.GetTimeZoneInfo(builder.Configuration["Hangfire:TimeZoneIana"] ?? "America/Bogota");

// Obligaciones mensuales
var cronObligations = builder.Configuration["Hangfire:CronObligations"] ?? "15 2 1 * *";
RecurringJob.AddOrUpdate<ObligationJobs>(
    "obligations-monthly",
    j => j.GenerateForCurrentMonthAsync(JobCancellationToken.Null),
    cronObligations,
    new RecurringJobOptions { TimeZone = tz, QueueName = "maintenance" }
);

// Contratos (expiración)
if (builder.Configuration.GetValue<bool>("Contracts:Expiration:Enabled"))
{
    var cronContracts = builder.Configuration["Contracts:Expiration:Cron"] ?? "*/10 * * * *";
    RecurringJob.AddOrUpdate<ContractJobs>(
        "contracts-expiration",
        j => j.RunExpirationSweepAsync(CancellationToken.None),
        cronContracts,
        new RecurringJobOptions { TimeZone = tz, QueueName = "default" }
    );
}

app.Run();
