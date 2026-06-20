using NutriTec.Contracts.Administracion;

namespace NutriTec.Application.Abstractions.Persistence;

public interface IAdministracionRepository
{
    Task<IReadOnlyCollection<ReporteCobroNutricionistaResponse>> GenerarReporteCobroAsync(
        decimal montoBasePorPaciente,
        bool incluirSinPacientes,
        CancellationToken cancellationToken);

    Task<decimal?> CalcularImcAsync(decimal pesoKg, decimal estaturaCm, CancellationToken cancellationToken);
}
