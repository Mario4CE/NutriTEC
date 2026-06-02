using Microsoft.Extensions.DependencyInjection;

namespace NutriTec.Application;

/// <summary>
/// Descripción:
/// Expone el punto de composición de la capa de aplicación compartida por las APIs de NutriTEC.
///
/// Entradas:
/// Recibe la colección de servicios configurada por el host ASP.NET Core.
///
/// Salidas:
/// Retorna la misma colección con los servicios de aplicación registrados.
///
/// Restricciones:
/// No debe registrar dependencias específicas de SQL Server ni de MongoDB.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Descripción:
    /// Registra los servicios y casos de uso compartidos de NutriTEC.
    ///
    /// Entradas:
    /// Recibe la colección de servicios del contenedor de inyección de dependencias.
    ///
    /// Salidas:
    /// Retorna la colección para permitir una configuración encadenada.
    ///
    /// Restricciones:
    /// Los servicios de negocio se agregarán gradualmente sin acoplar esta capa a una base de datos concreta.
    /// </summary>
    public static IServiceCollection AddNutriTecApplication(this IServiceCollection services)
    {
        return services;
    }
}