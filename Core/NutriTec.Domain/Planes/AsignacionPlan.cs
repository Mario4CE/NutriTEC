namespace NutriTec.Domain.Planes;

public sealed class AsignacionPlan
{
    public Guid Id { get; init; }
    public Guid IdPaciente { get; init; }
    public Guid IdPlan { get; init; }
    public string IdNutricionista { get; init; } = string.Empty;
    public DateOnly FechaInicio { get; init; }
    public DateOnly FechaFin { get; init; }
    public DateTime FechaAsignacionUtc { get; init; }
}