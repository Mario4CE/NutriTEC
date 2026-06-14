using NutriTec.Application;
using NutriTec.Infrastructure.Mongo;
using NutriTec.MongoApi.Middleware;

var builder = WebApplication.CreateBuilder(args);

/*
 * En esta API se ha optado por una arquitectura hexagonal, donde la lógica de negocio y la infraestructura están desacopladas.
 * La API Mongo actúa como un adaptador documental que se comunica con la lógica de negocio a través de interfaces.
 * Esto permite que la lógica de negocio sea independiente de la tecnología de almacenamiento utilizada, facilitando el mantenimiento y la escalabilidad.
 * Además, se han implementado manejadores de excepciones personalizados para mejorar la gestión de errores y proporcionar respuestas más claras a los clientes.
 */

// La API Mongo compone únicamente la lógica compartida y el adaptador documental.
builder.Services.AddNutriTecApplication();
builder.Services.AddNutriTecMongoInfrastructure(builder.Configuration);

builder.Services.AddControllers();
builder.Services.AddAuthorization();
builder.Services.AddExceptionHandler<ArgumentExceptionHandler>();
builder.Services.AddProblemDetails();
builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseExceptionHandler();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
