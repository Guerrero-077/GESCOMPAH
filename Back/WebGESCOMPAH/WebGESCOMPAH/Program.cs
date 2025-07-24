using Business.CQRS.Behaviors;
using Business.CQRS.SecrutityAuthentication.Login;
using Business.Mapping;
using CloudinaryDotNet;
using Entity.DTOs.Interfaces;
using Entity.DTOs.Services;
using Entity.DTOs.Validations.Entity.DTOs.Validators.SecurityAuthentication;
using ExceptionHandling;
using FluentValidation;
using MediatR;
using WebGESCOMPAH.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// CORS, JWT, DB, Servicios personalizados
builder.Services.AddCustomCors(builder.Configuration);
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddApplicationServices();
builder.Services.AddDatabase(builder.Configuration);

// Validaciones y CQRS
builder.Services.AddScoped<IValidatorService, ValidatorService>();
builder.Services.AddValidatorsFromAssemblyContaining<RegisterDtoValidator>();

builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssemblyContaining<LoginCommandHandler>());

// ✅ Configuración de Cloudinary como singleton
var cloudinaryConfig = builder.Configuration.GetSection("Cloudinary");
var cloudinary = new Cloudinary(new Account(
    cloudinaryConfig["CloudName"],
    cloudinaryConfig["ApiKey"],
    cloudinaryConfig["ApiSecret"]
));
cloudinary.Api.Secure = true;
builder.Services.AddSingleton(cloudinary);


// Mapster config (si aplica aquí)
MapsterConfig.Register();

// Construcción de la app
var app = builder.Build();

// Middleware y pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseGlobalExceptionHandling();
app.UseAuthentication();
app.UseCors();
app.UseAuthorization();
app.MapControllers();

app.Run();