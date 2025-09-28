using WebGESCOMPAH.Extensions.Composition;
using WebGESCOMPAH.Extensions.Infrastructure;
using WebGESCOMPAH.Extensions.Presentation;
using WebGESCOMPAH.Extensions.RealTime;

var builder = WebApplication.CreateBuilder(args);

// --------------------------
// CONFIGURACIÓN Y SERVICIOS
// --------------------------

// Controllers / Swagger
builder.Services.AddPresentationControllers();
builder.Services.AddCustomSwagger();

// Servicios de dominio (Business/Data) y módulos
builder.Services.AddApplicationServices();

// Infraestructura transversal (CORS, JWT, DB, Cloudinary, Hangfire, SignalR, Validations, Proxy/HTTPS, PDF warmup deps)
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

// --------------------------
// MIDDLEWARE GLOBAL (ORDEN)
// --------------------------

// APLICA LOS HEADERS DEL PROXY ANTES QUE NADA
app.UseForwardedHeaders();

app.UseMiddleware<ExceptionMiddleware>();
app.UseStaticFiles();

// Swagger con soporte a proxies
app.UseCustomSwagger();

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
app.MapAppSignalRHubs();

// Warmup Playwright/Chromium (PDF)
await app.UsePdfWarmupAsync();

// HANGFIRE Dashboard + Recurring Jobs
app.UseHangfireDashboardAndJobs(builder.Configuration);

app.Run();
