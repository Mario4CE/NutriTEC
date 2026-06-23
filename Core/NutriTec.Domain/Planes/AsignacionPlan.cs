namespace NutriTec.Domain.Planes;

public sealed class AsignacionPlan
{
    public int Id { get; init; }
    public int IdPaciente { get; init; }
    public int IdPlan { get; init; }
    public string IdNutricionista { get; init; } = string.Empty;
    public DateOnly FechaInicio { get; init; }
    public DateOnly FechaFin { get; init; }
    public DateTime FechaAsignacionUtc { get; init; }
}