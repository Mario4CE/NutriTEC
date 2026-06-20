namespace NutriTec.Application.Abstractions.Persistence.ObjetosSql;

public sealed record ReporteCobroNutricionistaSql(
    string CedulaNutricionista,
    string NombreNutricionista,
    string TipoCobro,
    int CantidadPacientes,
    decimal MontoBasePorPaciente,
    decimal Subtotal,
    decimal PorcentajeDescuento,
    decimal MontoDescuento,
    decimal TotalCobrar);
