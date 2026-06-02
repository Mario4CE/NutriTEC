using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NutriTec.Application.Abstractions.Persistence;
using NutriTec.Infrastructure.Sql.Persistence;
using NutriTec.Infrastructure.Sql.Repositories;

namespace NutriTec.Infrastructure.Sql;

/*
 * Descripción:
 * Expone el registro de dependencias exclusivas de la persistencia SQL Server.
 *
 * Entradas:
 * Recibe la colección de servicios y la configuración del host de la API SQL.
 *
 * Salidas:
 * Retorna la colección con Entity Framework Core configurado para SQL Server.
 *
 * Restricciones:
 * Este punto de composición solo debe invocarse desde NutriTec.SqlApi.
 */
public static class DependencyInjection
{
    /*
     * Descripción:
     * Registra el contexto relacional de NutriTEC con el proveedor de Entity Framework Core para SQL Server.
     *
     * Entradas:
     * Recibe la colección de servicios y una configuración que debe incluir la cadena NutriTecSqlServer.
     *
     * Salidas:
     * Retorna la colección para permitir una configuración encadenada.
     *
     * Restricciones:
     * La cadena de conexión debe suministrarse mediante configuración externa o placeholders de desarrollo; nunca debe codificarse como secreto en C#.
     */
    public static IServiceCollection AddNutriTecSqlInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("NutriTecSqlServer")
            ?? throw new InvalidOperationException(
                "No se configuró la cadena de conexión 'ConnectionStrings:NutriTecSqlServer'.");

        services.AddDbContext<NutriTecDbContext>(options => options.UseSqlServer(connectionString));
        services.AddScoped<IProductoRepository, ProductoSqlRepository>();

        return services;
    }
}
