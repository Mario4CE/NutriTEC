namespace NutriTec.Application.Abstractions.Persistence.ObjetosSql;

public sealed record MedidaUsuarioSql(
    int IdMedida,
    int IdUsuario,
    DateOnly Fecha,
    decimal Peso,
    decimal Imc);
