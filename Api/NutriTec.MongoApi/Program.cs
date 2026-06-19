using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using NutriTec.Application;
using NutriTec.Infrastructure.Mongo;
using NutriTec.MongoApi.Middleware;

var builder = WebApplication.CreateBuilder(args);

var jwtIssuer = builder.Configuration["Jwt:Issuer"];
var jwtAudience = builder.Configuration["Jwt:Audience"];
var jwtSecret = builder.Configuration["Jwt:Secret"];

if (builder.Environment.IsProduction() && string.IsNullOrWhiteSpace(jwtSecret))
{
    throw new InvalidOperationException("Jwt:Secret es obligatorio en producción.");
}

var jwtSigningKey = string.IsNullOrWhiteSpace(jwtSecret)
    ? "ConfigurationPlaceholderJwtSecretDoNotUseInProduction12345"
    : jwtSecret;

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
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtIssuer,
            ValidateAudience = true,
            ValidAudience = jwtAudience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSigningKey)),
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromMinutes(2)
        };
    });
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
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
