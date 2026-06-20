namespace NutriTec.Domain.Productos;

/*
 * Descripción:
 * Representa un producto alimenticio con su información nutricional básica y estado de aprobación.
 *
 * Entradas:
 * Recibe nombre, código de barras, valores nutricionales y fecha UTC de creación.
 *
 * Salidas:
 * Mantiene el agregado relacional que será persistido en SQL Server.
 *
 * Restricciones:
 * Los productos nuevos deben permanecer pendientes de aprobación hasta que un administrador los revise.
 */

public sealed class Producto
{
    public Guid Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string CodigoBarras { get; set; } = string.Empty;
    public decimal PorcionGramosMililitros { get; set; }
    public decimal Calorias { get; set; }
    public decimal Proteinas { get; set; }
    public decimal Carbohidratos { get; set; }
    public decimal Grasas { get; set; }
    public decimal SodioMiligramos { get; set; }
    public string? Vitaminas { get; set; }
    public decimal? CalcioMiligramos { get; set; }
    public decimal? HierroMiligramos { get; set; }
    public bool EstaAprobado { get; set; }
    public DateTime FechaCreacionUtc { get; set; }
}
