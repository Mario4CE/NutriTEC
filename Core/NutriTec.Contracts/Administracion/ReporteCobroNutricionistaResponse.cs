namespace NutriTec.Contracts.Administracion;

public sealed record ReporteCobroNutricionistaResponse(
    string CedulaNutricionista,
    string NombreNutricionista,
    string TipoCobro,
    int CantidadPacientes,
    decimal MontoBasePorPaciente,
    decimal Subtotal,
    decimal PorcentajeDescuento,
    decimal MontoDescuento,
    decimal TotalCobrar);
