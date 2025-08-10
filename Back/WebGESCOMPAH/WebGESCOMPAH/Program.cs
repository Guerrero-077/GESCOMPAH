using Business.CustomJWT;
using Business.Mapping;
using CloudinaryDotNet;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using Entity.DTOs.Interfaces;
using Entity.DTOs.Services;
using Entity.DTOs.Validations.Entity.DTOs.Validators.SecurityAuthentication;
using FluentValidation;
using Utilities.Helpers.CloudinaryHelper;
using WebGESCOMPAH.Extensions;
using WebGESCOMPAH.Middleware;
using WebGESCOMPAH.Middleware.Handlers;

var builder = WebApplication.CreateBuilder(args);

// --------------------------
// SERVICIOS (DI Container)
// --------------------------

// Controladores
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// CORS, JWT, DB, Servicios personalizados
builder.Services.AddCustomCors(builder.Configuration);
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddApplicationServices();
builder.Services.AddDatabase(builder.Configuration);

// Validaciones y CQRS
builder.Services.AddScoped<IValidatorService, ValidatorService>();
builder.Services.AddValidatorsFromAssemblyContaining<RegisterDtoValidator>();

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

// Mapster
MapsterConfig.Register();

// --------------------------
// MANEJO DE EXCEPCIONES
// --------------------------

// Registro de Handlers personalizados
builder.Services.AddScoped<IExceptionHandler, BusinessExceptionHandler>();
builder.Services.AddScoped<IExceptionHandler, EntityNotFoundExceptionHandler>();
builder.Services.AddScoped<IExceptionHandler, UnauthorizedAccessHandler>();
builder.Services.AddScoped<IExceptionHandler, ExternalServiceExceptionHandler>();

// Registro del middleware como scoped
builder.Services.AddScoped<ExceptionMiddleware>();

// --------------------------
// APP Y PIPELINE HTTP
// --------------------------

var app = builder.Build();

// Swagger solo en desarrollo
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// ✅ Middleware de excepciones (desde el contenedor de DI para evitar problemas de scope)
app.UseMiddleware<ExceptionMiddleware>();

app.UseAuthentication();
app.UseCors();
app.UseAuthorization();

app.MapControllers();

app.Run();
