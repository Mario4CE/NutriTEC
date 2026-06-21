namespace NutriTec.Domain.Planes;

public sealed class ItemPlan
{
    public int Id { get; init; }
    public int IdPlan { get; init; }
    public TiempoComida TiempoComida { get; init; }
    public Guid IdProducto { get; init; }
    public decimal Porciones { get; init; }
}