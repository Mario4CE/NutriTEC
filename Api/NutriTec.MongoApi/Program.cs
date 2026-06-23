using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
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
var corsAllowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [];

const string CorsPolicyName = "RestrictedCors";

/*
 * En esta API se ha optado por una arquitectura hexagonal, donde la lógica de negocio y la infraestructura están desacopladas.
 * La API Mongo actúa como un adaptador documental que se comunica con la lógica de negocio a través de interfaces.
 * Esto permite que la lógica de negocio sea independiente de la tecnología de almacenamiento utilizada, facilitando el mantenimiento y la escalabilidad.
 * Además, se han implementado manejadores de excepciones personalizados para mejorar la gestión de errores y proporcionar respuestas más claras a los clientes.
 */

// La API Mongo compone únicamente la lógica compartida y el adaptador documental.
builder.Services.AddNutriTecMongoApplication();
builder.Services.AddNutriTecMongoInfrastructure(builder.Configuration);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddCors(options =>
{
    options.AddPolicy(CorsPolicyName, policy =>
    {
        policy
            .WithOrigins(corsAllowedOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});
builder.Services.AddSwaggerGen(options =>
{
    options.CustomSchemaIds(type => type.FullName?.Replace("+", ".") ?? type.Name);
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "NutriTEC Mongo API",
        Version = "v1",
        Description = "Endpoints documentales de NutriTEC para retroalimentaciones."
    });
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Pegue el JWT sin escribir la palabra Bearer. Ejemplo: eyJhbGciOi..."
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});
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

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseExceptionHandler();
app.UseHttpsRedirection();
app.UseCors(CorsPolicyName);
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.MapGet("/", () => Results.Ok(new
{
    message = "NutriTec Mongo API funcionando",
    status = "OK"
}));

app.MapGet("/health", () => Results.Ok("OK"));

app.Run();
