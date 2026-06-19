namespace NutriTec.Domain.Planes;

/*
 * Descripción:
 * Representa un plan de alimentación creado por un nutricionista, compuesto por
 * productos distribuidos entre los cinco tiempos de comida.
 *
 * Entradas:
 * Recibe el nutricionista creador, un nombre descriptivo y la colección de items del plan.
 *
 * Salidas:
 * Sirve como base para asignar el plan a un paciente y para calcular el total calórico.
 *
 * Restricciones:
 * El nombre no puede estar vacío; la validación se aplica desde la capa de aplicación.
 */
public sealed class Plan
{
    public Guid Id { get; init; }
    public Guid IdNutricionista { get; init; }
    public string Nombre { get; init; } = string.Empty;
    public DateTime FechaCreacionUtc { get; init; }
    public List<ItemPlan> Items { get; init; } = [];
}