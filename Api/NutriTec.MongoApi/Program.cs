using NutriTec.Application;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Añadir referencias de Inyección de Dependencias
builder.Services.AddApplicationServices();
// builder.Services.AddMongoInfrastructureServices(builder.Configuration); // Descomentar cuando implementes DI en Infra

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
