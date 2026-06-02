using NutriTec.Application;
using NutriTec.Infrastructure.Sql;

var builder = WebApplication.CreateBuilder(args);

// La API SQL compone únicamente la lógica compartida y el adaptador relacional.
builder.Services.AddNutriTecApplication();
builder.Services.AddNutriTecSqlInfrastructure(builder.Configuration);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
