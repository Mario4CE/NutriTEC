using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NutriTec.Application.Abstractions.Services;
using NutriTec.Contracts.Pacientes;
using NutriTec.Contracts.Responses;

namespace NutriTec.SqlApi.Controllers;

/*
 * Descripción:
 * Expone endpoints REST para que un nutricionista busque clientes y los asocie como pacientes.
 *
 * Entradas:
 * Recibe solicitudes HTTP autenticadas con rol Nutricionista y un servicio de aplicación.
 *
 * Salidas:
 * Devuelve respuestas HTTP estandarizadas mediante ApiResponse<T>.
 *
 * Restricciones:
 * No accede directamente a Entity Framework Core; el identificador del nutricionista se
 * obtiene del token JWT, nunca del cuerpo de la solicitud.
 */

[ApiController]
[Route("api/pacientes")]
[Authorize]
public sealed class PacientesController(IPacienteService service) : ControllerBase
{
    /*
     * Descripción:
     * Busca clientes candidatos a ser asociados como pacientes.
     * Entradas:
     * Recibe criterio de búsqueda por query y token de cancelación HTTP.
     * Salidas:
     * Devuelve HTTP 200 con coincidencias.
     * Restricciones:
     * El criterio se valida en Application.
     */

    [HttpGet("buscar-clientes")]
    public async Task<ActionResult<ApiResponse<IReadOnlyCollection<ClienteBusquedaResponse>>>> BuscarClientesAsync(
        [FromQuery] string criterio,
        CancellationToken cancellationToken)
    {
        var clientes = await service.BuscarClientesAsync(criterio, cancellationToken);
        return Ok(ApiResponse<IReadOnlyCollection<ClienteBusquedaResponse>>.SuccessResponse(clientes));
    }

    /*
     * Descripción:
     * Asocia un cliente como paciente del nutricionista autenticado.
     * Entradas:
     * Recibe DTO con el cliente a asociar y token de cancelación HTTP.
     * Salidas:
     * Devuelve HTTP 201 con la asociación creada.
     * Restricciones:
     * El nutricionista se obtiene del token, no del cuerpo de la solicitud.
     */

    [HttpPost]
    public async Task<ActionResult<ApiResponse<PacienteNutricionistaResponse>>> AsociarAsync(
        [FromBody] NutriTec.Contracts.Pacientes.AsociarPacienteRequest request,
        CancellationToken cancellationToken)
    {
        var idNutricionista = ObtenerIdUsuarioActual();
        var asociacion = await service.AsociarAsync(idNutricionista, request, cancellationToken);
        return StatusCode(StatusCodes.Status201Created, ApiResponse<PacienteNutricionistaResponse>.SuccessResponse(asociacion, "Paciente asociado."));
    }

    /*
     * Descripción:
     * Lista los pacientes asociados al nutricionista autenticado.
     * Entradas:
     * Recibe token de cancelación HTTP.
     * Salidas:
     * Devuelve HTTP 200 con la lista de pacientes.
     * Restricciones:
     * El nutricionista se obtiene del token.
     */

    [HttpGet]
    public async Task<ActionResult<ApiResponse<IReadOnlyCollection<PacienteNutricionistaResponse>>>> ListarAsync(CancellationToken cancellationToken)
    {
        var idNutricionista = ObtenerIdUsuarioActual();
        var pacientes = await service.ListarPorNutricionistaAsync(idNutricionista, cancellationToken);
        return Ok(ApiResponse<IReadOnlyCollection<PacienteNutricionistaResponse>>.SuccessResponse(pacientes));
    }

    /*
     * Descripción:
     * Elimina la asociación entre el nutricionista autenticado y un paciente.
     * Entradas:
     * Recibe identificador de la asociación y token de cancelación HTTP.
     * Salidas:
     * Devuelve HTTP 204 o HTTP 404.
     * Restricciones:
     * Delega persistencia al servicio.
     */

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DesasociarAsync(Guid id, CancellationToken cancellationToken)
    {
        return await service.DesasociarAsync(id, cancellationToken)
            ? NoContent()
            : NotFound(ApiResponse<object>.ErrorResponse("No se encontró la asociación solicitada."));
    }

    private string ObtenerIdUsuarioActual()
    {
        return User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub") ?? string.Empty;
    }
}