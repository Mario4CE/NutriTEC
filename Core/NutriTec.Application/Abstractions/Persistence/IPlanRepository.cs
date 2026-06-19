using NutriTec.Domain.Planes;

namespace NutriTec.Application.Abstractions.Persistence;

public interface IPlanRepository
{
    Task<Plan> CrearAsync(Plan plan, CancellationToken cancellationToken);

    Task<Plan?> ObtenerPorIdAsync(Guid idPlan, CancellationToken cancellationToken);

    Task<IReadOnlyCollection<Plan>> ListarPorNutricionistaAsync(string idNutricionista, CancellationToken cancellationToken);

    Task<bool> ActualizarAsync(Plan plan, CancellationToken cancellationToken);

    Task<bool> EliminarAsync(Guid idPlan, CancellationToken cancellationToken);

    Task<bool> PertenecePlanAAsync(Guid idPlan, string idNutricionista, CancellationToken cancellationToken);
}