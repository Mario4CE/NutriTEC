namespace NutriTec.Infrastructure.Sql.Persistence.Entities;

public sealed class PlanSql
{
    public Guid Id { get; set; }
    public string IdNutricionista { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public DateTime FechaCreacionUtc { get; set; }
    public List<ItemPlanSql> Items { get; set; } = [];
}

public sealed class ItemPlanSql
{
    public Guid Id { get; set; }
    public Guid IdPlan { get; set; }
    public int TiempoComida { get; set; }
    public Guid IdProducto { get; set; }
    public decimal Porciones { get; set; }
}
