using Microsoft.AspNetCore.Mvc;
using NutriTec.Application;
using NutriTec.Contracts.Responses;
using NutriTec.Infrastructure.Sql;
using NutriTec.SqlApi.Middleware;

var builder = WebApplication.CreateBuilder(args);

// La API SQL compone únicamente la lógica compartida y el adaptador relacional.
builder.Services.AddNutriTecApplication();
builder.Services.AddNutriTecSqlInfrastructure(builder.Configuration);

builder.Services.AddControllers();
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
