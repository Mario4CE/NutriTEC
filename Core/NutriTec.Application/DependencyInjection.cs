using Microsoft.Extensions.DependencyInjection;
using NutriTec.Application.Abstractions.Services;
using NutriTec.Application.Administracion;
using NutriTec.Application.Autenticacion;
using NutriTec.Application.Productos;
using NutriTec.Application.Retroalimentaciones;

namespace NutriTec.Application;

/*
 * Descripción:
 * Expone el punto de composición de la capa de aplicación compartida por las APIs de NutriTEC.
 * En otras palabras, aquí se registran los servicios de aplicación que no dependen de la infraestructura específica de cada API (SQL Server o MongoDB).
 *
 * Entradas:
 * Recibe la colección de servicios configurada por el host ASP.NET Core.
 *
 * Salidas:
 * Retorna la misma colección con los servicios de aplicación registrados.
 *
 * Restricciones:
 * No debe registrar dependencias específicas de SQL Server ni de MongoDB.
 */

public static class DependencyInjection
{


    /*
     * Descripción:
     * Registra los servicios y casos de uso compartidos de NutriTEC.
     *
     * Entradas:
     * Recibe la colección de servicios del contenedor de inyección de dependencias.
     *
     * Salidas:
     * Retorna la colección para permitir una configuración encadenada.
     *
     * Restricciones:
     * No registra repositorios concretos ni conexiones a bases de datos.
     */

    public static IServiceCollection AddNutriTecApplication(this IServiceCollection services)
    {
        services.AddScoped<IAdministracionService, AdministracionService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IProductoService, ProductoService>();
        services.AddScoped<IRetroalimentacionService, RetroalimentacionService>();

        return services;
    }
}
