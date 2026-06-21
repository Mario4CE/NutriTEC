namespace NutriTec.Contracts.ObjetosSql;

public sealed record RegistrarMedidaUsuarioResponse(
    int IdMedida,
    int IdUsuario,
    DateOnly Fecha,
    decimal Peso,
    decimal Imc);
