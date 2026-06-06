using Microsoft.AspNetCore.Mvc;
using NutriTec.Application.Abstractions.Services;
using NutriTec.Contracts.Productos;
using NutriTec.Contracts.Responses;

namespace NutriTec.SqlApi.Controllers;

/*
 * Descripción:
 * Expone endpoints REST administrativos para revisar productos alimenticios.
 * Se encarga de listar productos pendientes de aprobación y aprobarlos, delegando la lógica al servicio administrativo.
 * Los productos pendientes se identifican por su estado y solo pueden ser aprobados o negados, sin modificaciones adicionales.
 *
 * Entradas:
 * Recibe solicitudes HTTP, identificadores y un servicio administrativo.
 *
 * Salidas:
 * Devuelve respuestas estandarizadas con productos pendientes o confirmaciones.
 *
 * Restricciones:
 * No accede directamente a Entity Framework Core y no contiene reglas de negocio.
 */

[ApiController]
[Route("api/administracion")]
public sealed class AdministracionController(IAdministracionService service) : ControllerBase
{


    /*
     * Descripción: Consulta productos pendientes de aprobación.
     * Entradas: Recibe token de cancelación HTTP.
     * Salidas: Devuelve HTTP 200 con productos pendientes.
     * Restricciones: No modifica datos, solo consulta el estado de los productos para su revisión.
     */

    [HttpGet("productos/pendientes")]
    public async Task<ActionResult<ApiResponse<IReadOnlyCollection<ProductoResponse>>>> ListarProductosPendientesAsync(
        CancellationToken cancellationToken)
    {
        var productos = await service.ListarProductosPendientesAsync(cancellationToken);
        return Ok(ApiResponse<IReadOnlyCollection<ProductoResponse>>.SuccessResponse(productos));
    }



    /*
     * Descripción: Aprueba un producto pendiente.
     * Entradas: Recibe identificador y token de cancelación HTTP.
     * Salidas: Devuelve HTTP 204 al aprobar o HTTP 404 si no existe un producto pendiente con ese identificador.
     * Restricciones: No permite desaprobar productos ya que solo se pueden aprobar o dejar pendientes, sin modificaciones adicionales. Solo cambia el estado del producto a aprobado.
     */
    [HttpPut("productos/{idProducto:guid}/aprobacion")]
    public async Task<IActionResult> AprobarProductoAsync(Guid idProducto, CancellationToken cancellationToken)
    {
        return await service.AprobarProductoAsync(idProducto, cancellationToken)
            ? NoContent()
            : NotFound(ApiResponse<object>.ErrorResponse("No se encontró un producto pendiente con el identificador indicado."));
    }
}
