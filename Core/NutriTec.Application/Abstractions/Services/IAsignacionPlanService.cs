using NutriTec.Contracts.Planes;

namespace NutriTec.Application.Abstractions.Services;

public interface IAsignacionPlanService
{
    Task<AsignacionPlanResponse> AsignarAsync(string idNutricionista, AsignarPlanRequest request, CancellationToken cancellationToken);

    Task<AsignacionPlanResponse?> ObtenerVigentePorPacienteAsync(Guid idPaciente, CancellationToken cancellationToken);

    Task<IReadOnlyCollection<AsignacionPlanResponse>> ListarPorPacienteAsync(Guid idPaciente, CancellationToken cancellationToken);
}