using Microsoft.Extensions.DependencyInjection;
using NutriTec.Application.Abstractions.Persistence;
using NutriTec.Infrastructure.Sql.Repositories;

namespace NutriTec.Infrastructure.Sql;

public static class DependencyInjection
{
    public static IServiceCollection AddNutriTecSqlInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<IAuthRepository, AuthSqlRepository>();

        return services;
    }
}
