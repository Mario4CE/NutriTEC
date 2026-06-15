using NutriTec.Application;
using NutriTec.Infrastructure.Sql;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddNutriTecApplication();
builder.Services.AddNutriTecSqlInfrastructure(builder.Configuration);
builder.Services.AddControllers();
builder.Services.AddAuthorization();

var app = builder.Build();

app.UseAuthorization();
app.MapControllers();

app.Run();
