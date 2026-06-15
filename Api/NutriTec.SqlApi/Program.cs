using NutriTec.Application;
using NutriTec.Infrastructure.Sql;
using NutriTec.SqlApi.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddNutriTecApplication();
builder.Services.AddNutriTecSqlInfrastructure(builder.Configuration);
builder.Services.AddControllers();
builder.Services.AddAuthorization();

var app = builder.Build();

app.UseMiddleware<ErrorHandlingMiddleware>();
app.UseAuthorization();
app.MapControllers();

app.Run();
