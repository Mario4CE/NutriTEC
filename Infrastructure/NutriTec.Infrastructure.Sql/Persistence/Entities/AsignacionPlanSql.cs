namespace NutriTec.Infrastructure.Sql.Persistence.Entities;

public sealed class AsignacionPlanSql
{
    public Guid Id { get; set; }
    public Guid IdPaciente { get; set; }
    public Guid IdPlan { get; set; }
    public string IdNutricionista { get; set; } = string.Empty;
    public DateOnly FechaInicio { get; set; }
    public DateOnly FechaFin { get; set; }
    public DateTime FechaAsignacionUtc { get; set; }
}