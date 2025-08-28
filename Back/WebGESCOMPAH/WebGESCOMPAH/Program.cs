using FluentValidation.AspNetCore;
using CloudinaryDotNet;
using Entity.Domain.Models.Implements.SecurityAuthentication;
using Entity.DTOs.Interfaces;
using Entity.DTOs.Services;
using Entity.DTOs.Validations.SecurityAuthentication.Auth;
using FluentValidation;
using Utilities.Helpers.CloudinaryHelper;
using WebGESCOMPAH.Extensions;

var builder = WebApplication.CreateBuilder(args);

// --------------------------
// CONFIGURACIÓN Y SERVICIOS
// --------------------------

builder.Services.AddControllers()
.AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
});

builder.Services.AddSwaggerGen(c =>
{
    c.UseInlineDefinitionsForEnums(); // 👈 Swagger mostrará el dropdown con los nombres
}); 

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(); // Swagger siempre disponible

// CORS, JWT, DB, Servicios personalizados
builder.Services.AddCustomCors(builder.Configuration);
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));
builder.Services.Configure<CookieSettings>(builder.Configuration.GetSection("Cookie"));

builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddApplicationServices();
builder.Services.AddDatabase(builder.Configuration);

// Validaciones y CQRS
builder.Services.AddScoped<IValidatorService, ValidatorService>();
builder.Services.AddValidatorsFromAssemblyContaining<RegisterDtoValidator>();
builder.Services.AddFluentValidationAutoValidation();

// --------------------------
// CLOUDINARY CONFIG
// --------------------------
var cloudinaryConfig = builder.Configuration.GetSection("Cloudinary");
var cloudinary = new Cloudinary(new Account(
    cloudinaryConfig["CloudName"],
    cloudinaryConfig["ApiKey"],
    cloudinaryConfig["ApiSecret"]
));
cloudinary.Api.Secure = true;
builder.Services.AddSingleton(cloudinary);
builder.Services.AddScoped<CloudinaryUtility>();


var app = builder.Build();

// --------------------------
// MIDDLEWARE GLOBAL
// --------------------------

// Manejo de errores
app.UseMiddleware<ExceptionMiddleware>();

// Archivos estáticos
app.UseStaticFiles();

// Swagger
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "GESCOMPAH API v1");
    c.RoutePrefix = "swagger";
});

// ✅ AQUI DEBE IR CORS: antes de Auth
app.UseCors();

// HTTPS
app.UseHttpsRedirection();

// 🔐 Seguridad
app.UseAuthentication();
app.UseAuthorization();

// Ruteo
app.MapControllers();

app.Run();
