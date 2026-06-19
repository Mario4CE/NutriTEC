using NutriTec.Contracts.Planes;

namespace NutriTec.Application.Abstractions.Services;

public interface IPlanService
{
    Task<PlanResponse> CrearAsync(string idNutricionista, CrearPlanRequest request, CancellationToken cancellationToken);

    Task<PlanResponse?> ObtenerPorIdAsync(Guid idPlan, CancellationToken cancellationToken);

    Task<IReadOnlyCollection<PlanResponse>> ListarPorNutricionistaAsync(string idNutricionista, CancellationToken cancellationToken);

    Task<bool> ActualizarAsync(string idNutricionista, Guid idPlan, ActualizarPlanRequest request, CancellationToken cancellationToken);

    Task<bool> EliminarAsync(string idNutricionista, Guid idPlan, CancellationToken cancellationToken);
}