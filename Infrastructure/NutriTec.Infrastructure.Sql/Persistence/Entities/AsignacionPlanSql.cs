namespace NutriTec.Infrastructure.Sql.Persistence.Entities;

public sealed class AsignacionPlanSql
{
    public int IdAsignacion { get; set; }
    public int IdPlan { get; set; }
    public int IdUsuario { get; set; }
    public DateOnly FechaInicio { get; set; }
    public DateOnly FechaFin { get; set; }
}