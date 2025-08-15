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


// Controladores
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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



var app = builder.Build();

// --------------------------
// MANEJO DE EXCEPCIONES
// --------------------------

// Registro de Handlers personalizados

// Registro del middleware como scoped

// ✅ Middleware de excepciones (desde el contenedor de DI para evitar problemas de scope)
app.UseMiddleware<ExceptionMiddleware>();

// --------------------------
// APP Y PIPELINE HTTP
// --------------------------
// Swagger solo en desarrollo
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseCors();
app.UseAuthorization();

app.MapControllers();

app.Run();
