namespace NutriTec.Contracts.ObjetosSql;

public sealed record ProductoAprobadoSqlResponse(
    Guid IdProducto,
    string Nombre,
    string CodigoBarras,
    decimal Calorias,
    decimal Proteinas,
    decimal Carbohidratos,
    decimal Grasas,
    bool Aprobado,
    DateTime FechaCreacionUtc);
