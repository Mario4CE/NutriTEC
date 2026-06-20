using NutriTec.Contracts.ObjetosSql;
using NutriTec.Contracts.Productos;

namespace NutriTec.Application.Abstractions.Services;

/*
 * Descripción:
 * Define los casos de uso administrativos disponibles para revisar productos.
 * 
 * ¿Qué hace esto? Se encarga de las revisiones de los productos.
 *
 * Entradas:
 * Recibe identificadores y tokens de cancelación enviados por la API SQL.
 *
 * Salidas:
 * Devuelve productos pendientes o confirma aprobaciones.
 *
 * Restricciones:
 * Mantiene separadas las operaciones administrativas del CRUD general de productos.
 */

public interface IAdministracionService
{
    /*
     * Descripción: Lista los productos que todavía requieren aprobación.
     * Entradas: Recibe token de cancelación.
     * Salidas: Devuelve DTOs de productos pendientes.
     * Restricciones: No modifica datos.
     */

    Task<IReadOnlyCollection<ProductoResponse>> ListarProductosPendientesAsync(CancellationToken cancellationToken);

    /*
     * Descripción: Aprueba un producto pendiente.
     * Entradas: Recibe identificador del producto y token de cancelación.
     * Salidas: Devuelve verdadero cuando el producto existe y cambia a aprobado.
     * Restricciones: Un producto previamente aprobado no vuelve a modificarse.
     */

    Task<bool> AprobarProductoAsync(Guid idProducto, CancellationToken cancellationToken);

    Task<IReadOnlyCollection<ReporteCobroNutricionistaResponse>> GenerarReporteCobroAsync(
        decimal montoBasePorPaciente,
        bool incluirSinPacientes,
        CancellationToken cancellationToken);

    Task<CalcularImcResponse> CalcularImcAsync(decimal pesoKg, decimal estaturaCm, CancellationToken cancellationToken);
}
