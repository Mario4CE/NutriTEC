using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NutriTec.Application.Abstractions.Services;
using NutriTec.Contracts.Planes;
using NutriTec.Contracts.Responses;

namespace NutriTec.SqlApi.Controllers;

/*
 * Descripción:
 * Expone endpoints REST para que un nutricionista gestione planes de alimentación y
 * los asigne a sus pacientes por un periodo de tiempo determinado.
 *
 * Entradas:
 * Recibe solicitudes HTTP autenticadas con rol Nutricionista y servicios de aplicación.
 *
 * Salidas:
 * Devuelve respuestas HTTP estandarizadas mediante ApiResponse<T>.
 *
 * Restricciones:
 * No accede directamente a Entity Framework Core; la cédula del nutricionista se
 * obtiene del token JWT, nunca del cuerpo de la solicitud.
 */

[ApiController]
[Route("api/planes")]
[Authorize]
public sealed class PlanesController(IPlanService planService, IAsignacionPlanService asignacionService) : ControllerBase
{
    /*
     * Descripción: Crea un plan de alimentación para el nutricionista autenticado.
     * Entradas: Recibe DTO de creación y token de cancelación HTTP.
     * Salidas: Devuelve HTTP 201 con el plan creado, incluyendo el total calórico.
     * Restricciones: El nutricionista se obtiene del token, no del cuerpo de la solicitud.
     */

    [HttpPost]
    public async Task<ActionResult<ApiResponse<PlanResponse>>> CrearAsync(
        [FromBody] CrearPlanRequest request,
        CancellationToken cancellationToken)
    {
        var idNutricionista = ObtenerIdUsuarioActual();
        var plan = await planService.CrearAsync(idNutricionista, request, cancellationToken);
        return StatusCode(StatusCodes.Status201Created, ApiResponse<PlanResponse>.SuccessResponse(plan, "Plan creado."));
    }

    /*
     * Descripción: Lista los planes creados por el nutricionista autenticado.
     * Entradas: Recibe token de cancelación HTTP.
     * Salidas: Devuelve HTTP 200 con la lista de planes.
     * Restricciones: El nutricionista se obtiene del token.
     */

    [HttpGet]
    public async Task<ActionResult<ApiResponse<IReadOnlyCollection<PlanResponse>>>> ListarAsync(CancellationToken cancellationToken)
    {
        var idNutricionista = ObtenerIdUsuarioActual();
        var planes = await planService.ListarPorNutricionistaAsync(idNutricionista, cancellationToken);
        return Ok(ApiResponse<IReadOnlyCollection<PlanResponse>>.SuccessResponse(planes));
    }

    /*
     * Descripción: Consulta un plan por identificador.
     * Entradas: Recibe identificador y token de cancelación HTTP.
     * Salidas: Devuelve HTTP 200 o HTTP 404.
     * Restricciones: Delega reglas al servicio.
     */

    [HttpGet("{idPlan:int}")]
    public async Task<ActionResult<ApiResponse<PlanResponse>>> ObtenerPorIdAsync(int idPlan, CancellationToken cancellationToken)
    {
        var plan = await planService.ObtenerPorIdAsync(idPlan, cancellationToken);
        return plan is null
            ? NotFound(ApiResponse<PlanResponse>.ErrorResponse("No se encontró el plan solicitado."))
            : Ok(ApiResponse<PlanResponse>.SuccessResponse(plan));
    }

    /*
     * Descripción: Actualiza el nombre y los items de un plan existente del nutricionista autenticado.
     * Entradas: Recibe identificador, DTO de edición y token de cancelación HTTP.
     * Salidas: Devuelve HTTP 204 o HTTP 404.
     * Restricciones: El plan debe pertenecer al nutricionista autenticado.
     */

    [HttpPut("{idPlan:int}")]
    public async Task<IActionResult> ActualizarAsync(
        int idPlan,
        [FromBody] ActualizarPlanRequest request,
        CancellationToken cancellationToken)
    {
        var idNutricionista = ObtenerIdUsuarioActual();
        return await planService.ActualizarAsync(idNutricionista, idPlan, request, cancellationToken)
            ? NoContent()
            : NotFound(ApiResponse<object>.ErrorResponse("No se encontró el plan solicitado."));
    }

    /*
     * Descripción: Elimina un plan de alimentación del nutricionista autenticado.
     * Entradas: Recibe identificador y token de cancelación HTTP.
     * Salidas: Devuelve HTTP 204 o HTTP 404.
     * Restricciones: El plan debe pertenecer al nutricionista autenticado.
     */

    [HttpDelete("{idPlan:int}")]
    public async Task<IActionResult> EliminarAsync(int idPlan, CancellationToken cancellationToken)
    {
        var idNutricionista = ObtenerIdUsuarioActual();
        return await planService.EliminarAsync(idNutricionista, idPlan, cancellationToken)
            ? NoContent()
            : NotFound(ApiResponse<object>.ErrorResponse("No se encontró el plan solicitado."));
    }

    /*
     * Descripción: Asigna un plan a un paciente del nutricionista autenticado para un periodo de tiempo.
     * Entradas: Recibe DTO de asignación y token de cancelación HTTP.
     * Salidas: Devuelve HTTP 201 con la asignación creada.
     * Restricciones: El paciente debe estar asociado al nutricionista y el plan debe pertenecerle.
     */

    [HttpPost("asignaciones")]
    public async Task<ActionResult<ApiResponse<AsignacionPlanResponse>>> AsignarAsync(
        [FromBody] AsignarPlanRequest request,
        CancellationToken cancellationToken)
    {
        var idNutricionista = ObtenerIdUsuarioActual();
        var asignacion = await asignacionService.AsignarAsync(idNutricionista, request, cancellationToken);
        return StatusCode(StatusCodes.Status201Created, ApiResponse<AsignacionPlanResponse>.SuccessResponse(asignacion, "Plan asignado."));
    }

    /*
     * Descripción: Consulta la asignación de plan vigente para un paciente.
     * Entradas: Recibe identificador del paciente y token de cancelación HTTP.
     * Salidas: Devuelve HTTP 200 o HTTP 404.
     * Restricciones: No modifica datos.
     */

    [HttpGet("asignaciones/vigente/{idPaciente:int}")]
    public async Task<ActionResult<ApiResponse<AsignacionPlanResponse>>> ObtenerVigenteAsync(int idPaciente, CancellationToken cancellationToken)
    {
        var asignacion = await asignacionService.ObtenerVigentePorPacienteAsync(idPaciente, cancellationToken);
        return asignacion is null
            ? NotFound(ApiResponse<AsignacionPlanResponse>.ErrorResponse("El paciente no tiene un plan vigente."))
            : Ok(ApiResponse<AsignacionPlanResponse>.SuccessResponse(asignacion));
    }

    /*
     * Descripción: Lista el historial de asignaciones de plan de un paciente.
     * Entradas: Recibe identificador del paciente y token de cancelación HTTP.
     * Salidas: Devuelve HTTP 200 con el historial de asignaciones.
     * Restricciones: No modifica datos.
     */

    [HttpGet("asignaciones/paciente/{idPaciente:int}")]
    public async Task<ActionResult<ApiResponse<IReadOnlyCollection<AsignacionPlanResponse>>>> ListarPorPacienteAsync(
        int idPaciente,
        CancellationToken cancellationToken)
    {
        var asignaciones = await asignacionService.ListarPorPacienteAsync(idPaciente, cancellationToken);
        return Ok(ApiResponse<IReadOnlyCollection<AsignacionPlanResponse>>.SuccessResponse(asignaciones));
    }

    private string ObtenerIdUsuarioActual()
    {
        return User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub") ?? string.Empty;
    }
}