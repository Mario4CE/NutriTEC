using Microsoft.AspNetCore.Mvc;
using NutriTec.Application;
using NutriTec.Contracts.Responses;
using NutriTec.Infrastructure.Sql;
using NutriTec.SqlApi.Middleware;

var builder = WebApplication.CreateBuilder(args);

/*
 * En esta parte de la aplicación, solo se registra la lógica compartida y el adaptador relacional.
 * La lógica de negocio, los casos de uso y las entidades se encuentran en el proyecto NutriTec.Application, que es independiente de la infraestructura.
 * El API SQL se encarga únicamente de exponer los endpoints y manejar las solicitudes HTTP, delegando la lógica de negocio a la capa de aplicación y el acceso a datos a la capa de infraestructura SQL.
*/

// La API SQL compone únicamente la lógica compartida y el adaptador relacional.
builder.Services.AddNutriTecApplication();
builder.Services.AddNutriTecSqlInfrastructure(builder.Configuration);

builder.Services.AddControllers();
builder.Services.AddAuthorization();
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var errors = context.ModelState.Values
            .SelectMany(value => value.Errors)
            .Select(error => error.ErrorMessage)
            .ToList();

        return new BadRequestObjectResult(
            ApiResponse<object>.ErrorResponse("La solicitud contiene datos inválidos.", errors));
    };
});
builder.Services.AddExceptionHandler<ApplicationExceptionHandler>();
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
