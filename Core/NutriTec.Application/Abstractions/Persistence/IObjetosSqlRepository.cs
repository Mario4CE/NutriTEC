using NutriTec.Application.Abstractions.Persistence.ObjetosSql;

namespace NutriTec.Application.Abstractions.Persistence;

public interface IObjetosSqlRepository
{
    Task<IReadOnlyCollection<ReporteCobroNutricionistaSql>> ObtenerReporteCobroNutricionistasAsync(
        decimal montoBasePorPaciente,
        bool incluirSinPacientes,
        CancellationToken cancellationToken);

    Task<ProductoAprobadoSql?> AprobarProductoConProcedimientoAsync(Guid idProducto, CancellationToken cancellationToken);

    Task<int> AsignarPlanPacienteAsync(int idPlan, int idUsuario, DateOnly fechaInicio, DateOnly fechaFin, CancellationToken cancellationToken);

    Task<MedidaUsuarioSql> RegistrarMedidaUsuarioAsync(
        int idUsuario,
        DateOnly fecha,
        decimal pesoKg,
        decimal estaturaCm,
        decimal? cintura,
        decimal? cuello,
        decimal? caderas,
        decimal? pctMusculo,
        decimal? pctGrasa,
        CancellationToken cancellationToken);

    Task<decimal?> CalcularImcAsync(decimal pesoKg, decimal estaturaCm, CancellationToken cancellationToken);

    Task<decimal> ObtenerTotalCaloriasPlanAsync(int idPlan, CancellationToken cancellationToken);
}
