namespace NutriTec.Infrastructure.Sql.Persistence.Entities;

public sealed class PlanAlimentacionSql
{
    public int IdPlan { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string CedulaNutricionista { get; set; } = string.Empty;
    public decimal TotalCalorias { get; set; }
    public List<TiempoComidaPlanSql> Tiempos { get; set; } = [];
}

public sealed class TiempoComidaPlanSql
{
    public int IdTiempo { get; set; }
    public int IdPlan { get; set; }
    public string TipoComida { get; set; } = string.Empty;
    public List<PlanProductoSql> Productos { get; set; } = [];
}

public sealed class PlanProductoSql
{
    public int IdTiempo { get; set; }
    public Guid IdProducto { get; set; }
    public decimal CantidadPorciones { get; set; }
}