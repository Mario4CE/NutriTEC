using NutriTec.Contracts.Productos;

namespace NutriTec.Application.Abstractions.Services;

/*
 * Descripción:
 * Define los casos de uso iniciales para administrar productos alimenticios.
 *
 * Entradas:
 * Recibe DTOs, identificadores y criterios de búsqueda enviados por la API SQL.
 *
 * Salidas:
 * Devuelve DTOs de productos y resultados de modificación.
 *
 * Restricciones:
 * No incluye aprobación administrativa; esa responsabilidad se agregará como caso de uso separado.
 */
public interface IProductoService
{
    /*
     * Descripción: Registra un producto pendiente de aprobación.
     * Entradas: Recibe DTO de creación y token de cancelación.
     * Salidas: Devuelve el producto creado como DTO.
     * Restricciones: El código de barras debe ser único.
     */
    Task<ProductoResponse> CrearAsync(CrearProductoRequest request, CancellationToken cancellationToken);

    /*
     * Descripción: Consulta un producto por identificador.
     * Entradas: Recibe identificador y token de cancelación.
     * Salidas: Devuelve el DTO encontrado o nulo.
     * Restricciones: El identificador no puede ser vacío.
     */
    Task<ProductoResponse?> ObtenerPorIdAsync(Guid idProducto, CancellationToken cancellationToken);

    /*
     * Descripción: Lista productos registrados.
     * Entradas: Recibe token de cancelación.
     * Salidas: Devuelve una colección de DTOs.
     * Restricciones: No modifica datos.
     */
    Task<IReadOnlyCollection<ProductoResponse>> ListarAsync(CancellationToken cancellationToken);

    /*
     * Descripción: Busca productos por coincidencia de nombre.
     * Entradas: Recibe criterio y token de cancelación.
     * Salidas: Devuelve una colección de DTOs coincidentes.
     * Restricciones: El criterio no puede estar vacío.
     */
    Task<IReadOnlyCollection<ProductoResponse>> BuscarPorNombreAsync(string nombre, CancellationToken cancellationToken);

    /*
     * Descripción: Consulta un producto por código de barras.
     * Entradas: Recibe código y token de cancelación.
     * Salidas: Devuelve el DTO encontrado o nulo.
     * Restricciones: El código no puede estar vacío.
     */
    Task<ProductoResponse?> ObtenerPorCodigoBarrasAsync(string codigoBarras, CancellationToken cancellationToken);

    /*
     * Descripción: Edita un producto existente.
     * Entradas: Recibe identificador, DTO de edición y token de cancelación.
     * Salidas: Devuelve verdadero cuando actualiza el producto.
     * Restricciones: No modifica el estado de aprobación.
     */
    Task<bool> ActualizarAsync(Guid idProducto, ActualizarProductoRequest request, CancellationToken cancellationToken);

    /*
     * Descripción: Elimina un producto existente.
     * Entradas: Recibe identificador y token de cancelación.
     * Salidas: Devuelve verdadero cuando elimina el producto.
     * Restricciones: El identificador no puede ser vacío.
     */
    Task<bool> EliminarAsync(Guid idProducto, CancellationToken cancellationToken);
}
