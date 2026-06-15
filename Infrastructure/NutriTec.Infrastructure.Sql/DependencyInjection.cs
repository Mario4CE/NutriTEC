using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NutriTec.Application.Abstractions.Persistence;
using NutriTec.Application.Abstractions.Services;
using NutriTec.Infrastructure.Sql.Persistence;
using NutriTec.Infrastructure.Sql.Repositories;
using NutriTec.Infrastructure.Sql.Security;

namespace NutriTec.Infrastructure.Sql;

public static class DependencyInjection
{
    public static IServiceCollection AddNutriTecSqlInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<NutriTecDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("NutriTec")));

        services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));
        services.AddScoped<IAuthRepository, AuthSqlRepository>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<ITokenService, JwtTokenService>();

        return services;
    }
}
