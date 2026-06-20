using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NutriTec.Application.Abstractions.Services;
using NutriTec.Contracts.ObjetosSql;
using NutriTec.Contracts.Responses;

namespace NutriTec.SqlApi.Controllers;

[ApiController]
[Route("api/sql-programable")]
[Authorize(Policy = "Administrador")]
public sealed class ObjetosSqlController(IObjetosSqlService service) : ControllerBase
{
    [HttpGet("stored-procedures/reporte-cobro-nutricionistas")]
    public async Task<ActionResult<ApiResponse<IReadOnlyCollection<ReporteCobroNutricionistaResponse>>>> ObtenerReporteCobroNutricionistasAsync(
        [FromQuery] decimal montoBasePorPaciente,
        [FromQuery] bool incluirSinPacientes = true,
        CancellationToken cancellationToken = default)
    {
        var reporte = await service.ObtenerReporteCobroNutricionistasAsync(montoBasePorPaciente, incluirSinPacientes, cancellationToken);
        return Ok(ApiResponse<IReadOnlyCollection<ReporteCobroNutricionistaResponse>>.SuccessResponse(reporte));
    }

    [HttpPut("stored-procedures/productos/{idProducto:guid}/aprobacion")]
    public async Task<ActionResult<ApiResponse<ProductoAprobadoSqlResponse>>> AprobarProductoConProcedimientoAsync(
        Guid idProducto,
        CancellationToken cancellationToken)
    {
        var producto = await service.AprobarProductoConProcedimientoAsync(idProducto, cancellationToken);
        return producto is null
            ? NotFound(ApiResponse<ProductoAprobadoSqlResponse>.ErrorResponse("No se encontró el producto indicado."))
            : Ok(ApiResponse<ProductoAprobadoSqlResponse>.SuccessResponse(producto, "Producto aprobado mediante procedimiento almacenado."));
    }

    [HttpPost("stored-procedures/planes/{idPlan:int}/pacientes/{idUsuario:int}/asignaciones")]
    public async Task<ActionResult<ApiResponse<AsignarPlanPacienteResponse>>> AsignarPlanPacienteAsync(
        int idPlan,
        int idUsuario,
        [FromBody] AsignarPlanPacienteRequest request,
        CancellationToken cancellationToken)
    {
        var asignacion = await service.AsignarPlanPacienteAsync(idPlan, idUsuario, request, cancellationToken);
        return StatusCode(StatusCodes.Status201Created, ApiResponse<AsignarPlanPacienteResponse>.SuccessResponse(asignacion, "Plan asignado mediante procedimiento almacenado."));
    }

    [HttpPost("stored-procedures/usuarios/{idUsuario:int}/medidas")]
    public async Task<ActionResult<ApiResponse<RegistrarMedidaUsuarioResponse>>> RegistrarMedidaUsuarioAsync(
        int idUsuario,
        [FromBody] RegistrarMedidaUsuarioRequest request,
        CancellationToken cancellationToken)
    {
        var medida = await service.RegistrarMedidaUsuarioAsync(idUsuario, request, cancellationToken);
        return StatusCode(StatusCodes.Status201Created, ApiResponse<RegistrarMedidaUsuarioResponse>.SuccessResponse(medida, "Medida registrada mediante procedimiento almacenado."));
    }

    [HttpGet("functions/imc")]
    public async Task<ActionResult<ApiResponse<CalcularImcResponse>>> CalcularImcAsync(
        [FromQuery] decimal pesoKg,
        [FromQuery] decimal estaturaCm,
        CancellationToken cancellationToken)
    {
        var imc = await service.CalcularImcAsync(pesoKg, estaturaCm, cancellationToken);
        return Ok(ApiResponse<CalcularImcResponse>.SuccessResponse(imc));
    }

    [HttpGet("functions/planes/{idPlan:int}/total-calorias")]
    public async Task<ActionResult<ApiResponse<TotalCaloriasPlanResponse>>> ObtenerTotalCaloriasPlanAsync(
        int idPlan,
        CancellationToken cancellationToken)
    {
        var total = await service.ObtenerTotalCaloriasPlanAsync(idPlan, cancellationToken);
        return Ok(ApiResponse<TotalCaloriasPlanResponse>.SuccessResponse(total));
    }
}
