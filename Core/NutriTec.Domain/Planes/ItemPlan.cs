namespace NutriTec.Domain.Planes;

/*
 * Descripción:
 * Representa un producto asignado a un tiempo de comida específico dentro de un plan
 * de alimentación.
 *
 * Entradas:
 * Recibe el plan al que pertenece, el tiempo de comida, el producto y la cantidad de porciones.
 *
 * Salidas:
 * Permite calcular el total nutricional del plan al combinarse con los datos del producto.
 *
 * Restricciones:
 * La cantidad de porciones debe ser mayor que cero; la validación se aplica desde Application.
 */
public sealed class ItemPlan
{
    public Guid Id { get; init; }
    public Guid IdPlan { get; init; }
    public TiempoComida TiempoComida { get; init; }
    public Guid IdProducto { get; init; }
    public decimal Porciones { get; init; }
}