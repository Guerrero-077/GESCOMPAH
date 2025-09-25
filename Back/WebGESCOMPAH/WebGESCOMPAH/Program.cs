using Business.Interfaces.PDF;
using Business.Services.Utilities.PDF;
using CloudinaryDotNet;
using Entity.Domain.Models.Implements.SecurityAuthentication;
using Entity.DTOs.Validations.SecurityAuthentication.Auth;
using Entity.Infrastructure.Binder; 
using FluentValidation;
using FluentValidation.AspNetCore;
using Hangfire;
using Hangfire.SqlServer;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using System.Text.Json;
using System.Text.Json.Serialization;
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
builder.Services
    .AddControllers(options =>
    {
        options.ModelBinderProviders.Insert(0, new FlexibleDecimalModelBinderProvider());
    })
    .AddJsonOptions(o =>
    {
        // permitir números como string en JSON
        o.JsonSerializerOptions.NumberHandling = JsonNumberHandling.AllowReadingFromString;
        // tolerar comas finales (ej. { "a":1, }) y comentarios si llegan desde herramientas
        o.JsonSerializerOptions.AllowTrailingCommas = true;
        o.JsonSerializerOptions.ReadCommentHandling = JsonCommentHandling.Skip;
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
builder.Services.AddScoped<ContractJobs>();

// Respuesta clara para errores de ModelState (400)
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = ctx =>
    {
        var problem = new ValidationProblemDetails(ctx.ModelState)
        {
            Status = StatusCodes.Status400BadRequest,
            Title = "Errores de validación en la solicitud.",
            Detail = "Revisa el formato del JSON y los campos obligatorios (evita comas finales y usa fechas YYYY-MM-DD).",
            Type = "https://tools.ietf.org/html/rfc9110#section-15.5.1",
            Instance = ctx.HttpContext.Request.Path
        };
        problem.Extensions["traceId"] = ctx.HttpContext.TraceIdentifier;
        return new BadRequestObjectResult(problem);
    };
});

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

// ==========================
// PROXY/HTTPS (CLAVE CON NGROK)
// ==========================

// 1) Forwarded Headers: confiar en los encabezados del proxy (ngrok) para X-Forwarded-Proto.
//    Nota: desde .NET 8.0.17/9.0.6 debes confiar explícitamente en proxies; en dev limpiamos listas.
//    En PROD, configura KnownNetworks/KnownProxies con valores específicos.
builder.Services.Configure<ForwardedHeadersOptions>(o =>
{
    o.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    o.KnownNetworks.Clear(); // SOLO DEV
    o.KnownProxies.Clear();  // SOLO DEV
});

// 2) Redirección HTTPS: fija el puerto a 443 para que NUNCA redirija a :7165 en el host de ngrok.
builder.Services.Configure<HttpsRedirectionOptions>(o => o.HttpsPort = 443);

var app = builder.Build();

// --------------------------
// MIDDLEWARE GLOBAL (ORDEN)
// --------------------------

// ⇩⇩⇩  APLICA LOS HEADERS DEL PROXY ANTES QUE NADA
app.UseForwardedHeaders();

app.UseMiddleware<ExceptionMiddleware>();
app.UseStaticFiles();

// Swagger: fija el "server" al host/esquema reales (ngrok) con los headers reenviados
app.UseSwagger(c =>
{
    c.PreSerializeFilters.Add((swagger, req) =>
    {
        var scheme = req.Headers["X-Forwarded-Proto"].FirstOrDefault() ?? req.Scheme;
        var host = req.Headers["X-Forwarded-Host"].FirstOrDefault() ?? req.Host.Value;
        var basePath = req.PathBase.HasValue ? req.PathBase.Value : string.Empty;

        swagger.Servers = new List<OpenApiServer> {
            new OpenApiServer { Url = $"{scheme}://{host}{basePath}" }
        };
    });
});

app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "GESCOMPH API v1");
    c.RoutePrefix = "swagger";
});

// CORS (antes de auth para preflights)
app.UseCors();

// Redirección a HTTPS (ahora que ya “vemos” X-Forwarded-Proto, no habrá bucles ni puertos raros)
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Migraciones multi-DB en arranque
MigrationManager.MigrateAllDatabases(app.Services, builder.Configuration);

// SignalR hubs
app.MapHub<ContractsHub>("/api/hubs/contracts");
app.MapHub<SecurityHub>("/api/hubs/security");

// Warmup Playwright/Chromium (PDF)
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

// HANGFIRE Dashboard + Recurring Jobs
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
