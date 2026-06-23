namespace NutriTec.Domain.Planes;

public sealed class Plan
{
    public int Id { get; init; }
    public string IdNutricionista { get; init; } = string.Empty;
    public string Nombre { get; init; } = string.Empty;
    public DateTime FechaCreacionUtc { get; init; }
    public List<ItemPlan> Items { get; init; } = [];
}