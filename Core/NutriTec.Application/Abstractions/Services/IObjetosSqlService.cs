using NutriTec.Contracts.ObjetosSql;

namespace NutriTec.Application.Abstractions.Services;

public interface IObjetosSqlService
{
    Task<IReadOnlyCollection<ReporteCobroNutricionistaResponse>> ObtenerReporteCobroNutricionistasAsync(
        decimal montoBasePorPaciente,
        bool incluirSinPacientes,
        CancellationToken cancellationToken);

    Task<ProductoAprobadoSqlResponse?> AprobarProductoConProcedimientoAsync(Guid idProducto, CancellationToken cancellationToken);

    Task<AsignarPlanPacienteResponse> AsignarPlanPacienteAsync(
        int idPlan,
        int idUsuario,
        AsignarPlanPacienteRequest request,
        CancellationToken cancellationToken);

    Task<RegistrarMedidaUsuarioResponse> RegistrarMedidaUsuarioAsync(
        int idUsuario,
        RegistrarMedidaUsuarioRequest request,
        CancellationToken cancellationToken);

    Task<CalcularImcResponse> CalcularImcAsync(decimal pesoKg, decimal estaturaCm, CancellationToken cancellationToken);

    Task<TotalCaloriasPlanResponse> ObtenerTotalCaloriasPlanAsync(int idPlan, CancellationToken cancellationToken);
}
