using Microsoft.AspNetCore.Mvc;
using NutriTec.Application.Abstractions.Services;
using NutriTec.Contracts.Responses;
using NutriTec.Contracts.Retroalimentaciones;

namespace NutriTec.MongoApi.Controllers;

/*
 * Descripción:
 * Expone endpoints REST para crear, consultar y responder foros de retroalimentación almacenados en MongoDB.
 *
 * Entradas:
 * Recibe solicitudes HTTP, identificadores de ruta, DTOs y un servicio de aplicación.
 *
 * Salidas:
 * Devuelve respuestas HTTP estandarizadas mediante ApiResponse<T>.
 *
 * Restricciones:
 * No accede directamente a MongoDB ni contiene reglas de negocio.
 */
[ApiController]
[Route("api/retroalimentaciones")]
public sealed class RetroalimentacionesController(IRetroalimentacionService service) : ControllerBase
{
    /*
     * Descripción:
     * Crea un foro de retroalimentación con su primer mensaje.
     *
     * Entradas:
     * Recibe el DTO de creación y token de cancelación HTTP.
     *
     * Salidas:
     * Devuelve HTTP 201 con la retroalimentación creada.
     *
     * Restricciones:
     * Delega validaciones y persistencia al servicio de aplicación.
     */
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<RetroalimentacionResponse>), StatusCodes.Status201Created)]
    public async Task<ActionResult<ApiResponse<RetroalimentacionResponse>>> CrearAsync(
        [FromBody] CrearRetroalimentacionRequest request,
        CancellationToken cancellationToken)
    {
        var retroalimentacion = await service.CrearAsync(request, cancellationToken);
        return StatusCode(
            StatusCodes.Status201Created,
            ApiResponse<RetroalimentacionResponse>.SuccessResponse(retroalimentacion, "Retroalimentación creada."));
    }

    /*
     * Descripción:
     * Consulta los foros correspondientes a un paciente.
     *
     * Entradas:
     * Recibe el identificador del paciente y token de cancelación HTTP.
     *
     * Salidas:
     * Devuelve HTTP 200 con una colección de retroalimentaciones.
     *
     * Restricciones:
     * No consulta MongoDB directamente.
     */
    [HttpGet("pacientes/{idPaciente:guid}")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyCollection<RetroalimentacionResponse>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IReadOnlyCollection<RetroalimentacionResponse>>>> ObtenerPorPacienteAsync(
        Guid idPaciente,
        CancellationToken cancellationToken)
    {
        var retroalimentaciones = await service.ObtenerPorPacienteAsync(idPaciente, cancellationToken);
        return Ok(ApiResponse<IReadOnlyCollection<RetroalimentacionResponse>>.SuccessResponse(retroalimentaciones));
    }

    /*
     * Descripción:
     * Consulta los foros correspondientes a un nutricionista.
     *
     * Entradas:
     * Recibe el identificador del nutricionista y token de cancelación HTTP.
     *
     * Salidas:
     * Devuelve HTTP 200 con una colección de retroalimentaciones.
     *
     * Restricciones:
     * No consulta MongoDB directamente.
     */
    [HttpGet("nutricionistas/{idNutricionista:guid}")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyCollection<RetroalimentacionResponse>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IReadOnlyCollection<RetroalimentacionResponse>>>> ObtenerPorNutricionistaAsync(
        Guid idNutricionista,
        CancellationToken cancellationToken)
    {
        var retroalimentaciones = await service.ObtenerPorNutricionistaAsync(idNutricionista, cancellationToken);
        return Ok(ApiResponse<IReadOnlyCollection<RetroalimentacionResponse>>.SuccessResponse(retroalimentaciones));
    }

    /*
     * Descripción:
     * Agrega una respuesta a un foro existente.
     *
     * Entradas:
     * Recibe identificador de foro, DTO de respuesta y token de cancelación HTTP.
     *
     * Salidas:
     * Devuelve HTTP 204 cuando se actualiza o HTTP 404 cuando el foro no existe.
     *
     * Restricciones:
     * La escritura atómica se delega al repositorio Mongo.
     */
    [HttpPost("{idRetroalimentacion:guid}/mensajes")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ResponderAsync(
        Guid idRetroalimentacion,
        [FromBody] ResponderRetroalimentacionRequest request,
        CancellationToken cancellationToken)
    {
        var actualizada = await service.ResponderAsync(idRetroalimentacion, request, cancellationToken);
        if (!actualizada)
        {
            return NotFound(ApiResponse<object>.ErrorResponse("No se encontró la retroalimentación solicitada."));
        }

        return NoContent();
    }
}
