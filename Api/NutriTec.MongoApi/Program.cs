using NutriTec.Application;
using NutriTec.Infrastructure.Mongo;
using NutriTec.MongoApi.Middleware;

var builder = WebApplication.CreateBuilder(args);

// La API Mongo compone únicamente la lógica compartida y el adaptador documental.
builder.Services.AddNutriTecApplication();
builder.Services.AddNutriTecMongoInfrastructure(builder.Configuration);

builder.Services.AddControllers();
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
