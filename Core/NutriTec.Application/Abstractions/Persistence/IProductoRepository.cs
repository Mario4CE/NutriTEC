using NutriTec.Domain.Productos;

namespace NutriTec.Application.Abstractions.Persistence;

/*
 * Descripción:
 * Define las operaciones relacionales requeridas por el módulo de productos.
 *
 * Entradas:
 * Recibe agregados, identificadores y criterios de búsqueda del dominio.
 *
 * Salidas:
 * Devuelve productos persistidos o confirma modificaciones.
 *
 * Restricciones:
 * No expone Entity Framework Core para mantener Application desacoplada de SQL Server.
 */

public interface IProductoRepository
{
    /*
     * Descripción: Persiste un producto.
     * Entradas: Agregado y cancelación. 
     * Salidas: Agregado persistido. 
     * Restricciones: Recibe datos validados. 
     */

    Task<Producto> CrearAsync(Producto producto, CancellationToken cancellationToken);

    /* 
     * Descripción: Consulta por identificador.
     * Entradas: Identificador y cancelación.
     * Salidas: Producto o nulo. 
     * Restricciones: No modifica datos. 
     */

    Task<Producto?> ObtenerPorIdAsync(Guid idProducto, CancellationToken cancellationToken);

    /* Descripción: Lista productos.
     * Entradas: Cancelación. 
     * Salidas: Colección de productos. 
     * Restricciones: No modifica datos.
     */

    Task<IReadOnlyCollection<Producto>> ListarAsync(CancellationToken cancellationToken);

    /*
     * Descripción: Busca productos por nombre.
     * Entradas: Criterio y cancelación.
     * Salidas: Coincidencias.
     * Restricciones: No modifica datos. 
     */

    Task<IReadOnlyCollection<Producto>> BuscarPorNombreAsync(string nombre, CancellationToken cancellationToken);

    /* 
     * Descripción: Consulta por código de barras.
     * Entradas: Código y cancelación.
     * Salidas: Producto o nulo. 
     * Restricciones: No modifica datos.
     */
    Task<Producto?> ObtenerPorCodigoBarrasAsync(string codigoBarras, CancellationToken cancellationToken);

    /* Descripción: Verifica unicidad de código.
     * Entradas: Código, exclusión opcional y cancelación.
     * Salidas: Existencia. 
     * Restricciones: Permite excluir una edición.
     */

    Task<bool> ExisteCodigoBarrasAsync(string codigoBarras, Guid? idProductoExcluido, CancellationToken cancellationToken);

    /* Descripción: Persiste cambios.
     * Entradas: Agregado y cancelación.
     * Salidas: Confirmación. 
     * Restricciones: Recibe reglas ya aplicadas.
     */

    Task<bool> ActualizarAsync(Producto producto, CancellationToken cancellationToken);

    /* 
     * Descripción: Lista productos pendientes.
     * Entradas: Cancelación.
     * Salidas: Productos no aprobados.
     * Restricciones: No modifica datos.
     */

    Task<IReadOnlyCollection<Producto>> ListarPendientesAsync(CancellationToken cancellationToken);

    /*
     * Descripción: Aprueba un producto pendiente.
     * Entradas: Identificador y cancelación.
     * Salidas: Confirmación. 
     * Restricciones: No modifica productos ya aprobados. 
     */

    Task<bool> AprobarAsync(Guid idProducto, CancellationToken cancellationToken);

    /*
     * Descripción: Elimina por identificador.
     * Entradas: Identificador y cancelación. 
     * Salidas: Confirmación.
     * Restricciones: Devuelve falso si no existe.
     */

    Task<bool> EliminarAsync(Guid idProducto, CancellationToken cancellationToken);
}
