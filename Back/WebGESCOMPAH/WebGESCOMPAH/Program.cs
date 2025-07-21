
using Entity.DTOs.Interfaces;
using Entity.DTOs.Services;
using Entity.DTOs.Validations;
using Entity.DTOs.Validations.Entity.DTOs.Validators.SecurityAuthentication;
using FluentValidation;
using WebGESCOMPAH.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Cors

builder.Services.AddCustomCors(builder.Configuration);

//Jwt
builder.Services.AddJwtAuthentication(builder.Configuration);

//Services
builder.Services.AddApplicationServices(); 


//Database
builder.Services.AddDatabase(builder.Configuration);

builder.Services.AddScoped<IValidatorService, ValidatorService>();
builder.Services.AddValidatorsFromAssemblyContaining<RegisterDtoValidator>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}



app.UseHttpsRedirection();

app.UseAuthentication();// Usar autentificación de JWT

app.UseCors();

app.UseAuthorization();

app.MapControllers();

app.Run();
