namespace NutriTec.Application.Abstractions.Persistence.ObjetosSql;

public sealed record ProductoAprobadoSql(
    Guid IdProducto,
    string Nombre,
    string CodigoBarras,
    decimal Calorias,
    decimal Proteinas,
    decimal Carbohidratos,
    decimal Grasas,
    bool Aprobado,
    DateTime FechaCreacionUtc);
