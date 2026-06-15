using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using NutriTec.Application;
using NutriTec.Infrastructure.Sql;
using NutriTec.SqlApi.Middleware;

var builder = WebApplication.CreateBuilder(args);

var jwtIssuer = builder.Configuration["Jwt:Issuer"];
var jwtAudience = builder.Configuration["Jwt:Audience"];
var jwtSecret = builder.Configuration["Jwt:Secret"];
var jwtSigningKey = string.IsNullOrWhiteSpace(jwtSecret)
    ? "ConfigurationPlaceholderJwtSecretDoNotUseInProduction12345"
    : jwtSecret;

builder.Services.AddNutriTecApplication();
builder.Services.AddNutriTecSqlInfrastructure(builder.Configuration);
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
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Cliente", policy => policy.RequireRole("Cliente"));
    options.AddPolicy("Nutricionista", policy => policy.RequireRole("Nutricionista"));
    options.AddPolicy("Administrador", policy => policy.RequireRole("Administrador"));
});

var app = builder.Build();

app.UseMiddleware<ErrorHandlingMiddleware>();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
