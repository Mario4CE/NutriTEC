using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NutriTec.Application.Abstractions.Services;
using NutriTec.Contracts.Productos;
using NutriTec.Contracts.Responses;

namespace NutriTec.SqlApi.Controllers;

/*
 * Descripción:
 * Expone endpoints REST iniciales para gestionar productos alimenticios almacenados en SQL Server.
 * Los productos registrados a través de este controlador quedan pendientes de aprobación administrativa.
 * Los productos son visibles en listados y búsquedas, pero no se pueden aprobar desde este controlador.
 *
 * Entradas:
 * Recibe solicitudes HTTP, identificadores, criterios de búsqueda y un servicio de aplicación.
 *
 * Salidas:
 * Devuelve respuestas HTTP estandarizadas mediante ApiResponse<T>.
 *
 * Restricciones:
 * No accede directamente a Entity Framework Core ni implementa aprobación administrativa.
 */

[ApiController]
[Route("api/productos")]
public sealed class ProductosController(IProductoService service) : ControllerBase
{


    /*
     * Descripción:
     * Registra un producto pendiente de aprobación.
     * Entradas:
     * Recibe DTO y token de cancelación HTTP.
     * Salidas:
     * Devuelve HTTP 201 con el producto creado.
     * Restricciones:
     * Delega reglas al servicio.
     */

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<ApiResponse<ProductoResponse>>> CrearAsync(
        [FromBody] CrearProductoRequest request,
        CancellationToken cancellationToken)
    {
        var producto = await service.CrearAsync(request, cancellationToken);
        return StatusCode(StatusCodes.Status201Created, ApiResponse<ProductoResponse>.SuccessResponse(producto, "Producto creado."));
    }



    /*
     * Descripción:
     * Lista productos registrados.
     * Entradas:
     * Recibe token de cancelación HTTP.
     * Salidas:
     * Devuelve HTTP 200 con productos.
     * Restricciones:
     * No consulta EF Core directamente.
     */

    [HttpGet]
    public async Task<ActionResult<ApiResponse<IReadOnlyCollection<ProductoResponse>>>> ListarAsync(CancellationToken cancellationToken)
    {
        var productos = await service.ListarAsync(cancellationToken);
        return Ok(ApiResponse<IReadOnlyCollection<ProductoResponse>>.SuccessResponse(productos));
    }



    /*
     * Descripción:
     * Consulta un producto por identificador.
     * Entradas:
     * Recibe identificador y token de cancelación HTTP.
     * Salidas:
     * Devuelve HTTP 200 o HTTP 404.
     * Restricciones:
     * Delega reglas al servicio.
     */

    [HttpGet("{idProducto:guid}")]
    public async Task<ActionResult<ApiResponse<ProductoResponse>>> ObtenerPorIdAsync(Guid idProducto, CancellationToken cancellationToken)
    {
        var producto = await service.ObtenerPorIdAsync(idProducto, cancellationToken);
        return producto is null
            ? NotFound(ApiResponse<ProductoResponse>.ErrorResponse("No se encontró el producto solicitado."))
            : Ok(ApiResponse<ProductoResponse>.SuccessResponse(producto));
    }



    /*
     * Descripción:
     * Busca productos por nombre.
     * Entradas:
     * Recibe criterio de query y token de cancelación HTTP.
     * Salidas:
     * Devuelve HTTP 200 con coincidencias.
     * Restricciones:
     * El criterio se valida en Application.
     */

    [HttpGet("buscar")]
    public async Task<ActionResult<ApiResponse<IReadOnlyCollection<ProductoResponse>>>> BuscarPorNombreAsync(
        [FromQuery] string nombre,
        CancellationToken cancellationToken)
    {
        var productos = await service.BuscarPorNombreAsync(nombre, cancellationToken);
        return Ok(ApiResponse<IReadOnlyCollection<ProductoResponse>>.SuccessResponse(productos));
    }



    /*
     * Descripción:
     * Consulta un producto por código de barras.
     * Entradas:
     * Recibe código y token de cancelación HTTP.
     * Salidas:
     * Devuelve HTTP 200 o HTTP 404.
     * Restricciones:
     * Delega reglas al servicio.
     */

    [HttpGet("codigo-barras/{codigoBarras}")]
    public async Task<ActionResult<ApiResponse<ProductoResponse>>> ObtenerPorCodigoBarrasAsync(
        string codigoBarras,
        CancellationToken cancellationToken)
    {
        var producto = await service.ObtenerPorCodigoBarrasAsync(codigoBarras, cancellationToken);
        return producto is null
            ? NotFound(ApiResponse<ProductoResponse>.ErrorResponse("No se encontró el producto solicitado."))
            : Ok(ApiResponse<ProductoResponse>.SuccessResponse(producto));
    }



    /*
     * Descripción:
     * Edita un producto existente.
     * Entradas:
     * Recibe identificador, DTO y token de cancelación HTTP.
     * Salidas:
     * Devuelve HTTP 204 o HTTP 404.
     * Restricciones:
     * No permite aprobar productos.
     */

    [HttpPut("{idProducto:guid}")]
    [Authorize(Policy = "Administrador")]
    public async Task<IActionResult> ActualizarAsync(
        Guid idProducto,
        [FromBody] ActualizarProductoRequest request,
        CancellationToken cancellationToken)
    {
        return await service.ActualizarAsync(idProducto, request, cancellationToken)
            ? NoContent()
            : NotFound(ApiResponse<object>.ErrorResponse("No se encontró el producto solicitado."));
    }



    /*
     * Descripción:
     * Elimina un producto existente.
     * Entradas:
     * Recibe identificador y token de cancelación HTTP.
     * Salidas:
     * Devuelve HTTP 204 o HTTP 404.
     * Restricciones:
     * Delega persistencia al servicio.
     */

    [HttpDelete("{idProducto:guid}")]
    [Authorize(Policy = "Administrador")]
    public async Task<IActionResult> EliminarAsync(Guid idProducto, CancellationToken cancellationToken)
    {
        return await service.EliminarAsync(idProducto, cancellationToken)
            ? NoContent()
            : NotFound(ApiResponse<object>.ErrorResponse("No se encontró el producto solicitado."));
    }
}
