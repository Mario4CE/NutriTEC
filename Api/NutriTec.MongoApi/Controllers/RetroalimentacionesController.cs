using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NutriTec.Application.Abstractions.Services;
using NutriTec.Contracts.Responses;
using NutriTec.Contracts.Retroalimentaciones;

namespace NutriTec.MongoApi.Controllers;

/*
 * Descripción:
 * Expone endpoints REST para crear, consultar y responder foros de retroalimentación almacenados en MongoDB.
 * 
 * Retroalimentacion se refiere a los comentarios y discusiones entre pacientes y nutricionistas sobre planes alimenticios, dietas, etc.
 * La razon de usar MongoDB es su flexibilidad para manejar estructuras de datos anidadas y dinámicas, como los mensajes dentro de un foro de retroalimentación.
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
[Authorize]
public sealed class RetroalimentacionesController(IRetroalimentacionService service) : ControllerBase
{
    /*
     * Descripción:
     * Crea un foro de retroalimentación con su primer mensaje.
     * 
     * El foro se inicia con un mensaje del autor (paciente o nutricionista) y puede ser respondido posteriormente por la otra parte.
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
     * Los foros se filtran por el identificador del paciente, mostrando solo aquellos en los que el paciente es parte de la conversación (ya sea como autor o destinatario).
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

    /*
    * Descripción:
    * Endpoint alternativo que acepta el id_usuario como int (SQL Server)
    * y lo convierte internamente a Guid para consultar MongoDB.
    * Resuelve la incompatibilidad de tipos entre SQL Server (int) y MongoDB (Guid).
    */
    [HttpGet("pacientes/int/{idUsuario:int}")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyCollection<RetroalimentacionResponse>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IReadOnlyCollection<RetroalimentacionResponse>>>> ObtenerPorPacienteIntAsync(
        int idUsuario,
        CancellationToken cancellationToken)
    {
        var idGuid = new Guid($"00000000-0000-0000-0000-{idUsuario:D12}");
        var retroalimentaciones = await service.ObtenerPorPacienteAsync(idGuid, cancellationToken);
        return Ok(ApiResponse<IReadOnlyCollection<RetroalimentacionResponse>>.SuccessResponse(retroalimentaciones));
    }

    /*
    * Descripción:
    * Endpoint alternativo que acepta la cédula del nutricionista como string (SQL Server)
    * y la convierte internamente a Guid para consultar MongoDB.
    */
    [HttpGet("nutricionistas/cedula/{cedula}")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyCollection<RetroalimentacionResponse>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IReadOnlyCollection<RetroalimentacionResponse>>>> ObtenerPorNutricionistaCedulaAsync(
        string cedula,
        CancellationToken cancellationToken)
    {
        var cedulaNum = long.TryParse(cedula, out var num) ? num : 0;
        var idGuid = new Guid($"00000000-0000-0000-0000-{cedulaNum:D12}");
        var retroalimentaciones = await service.ObtenerPorNutricionistaAsync(idGuid, cancellationToken);
        return Ok(ApiResponse<IReadOnlyCollection<RetroalimentacionResponse>>.SuccessResponse(retroalimentaciones));
    }
}
