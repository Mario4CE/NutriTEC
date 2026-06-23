namespace NutriTec.Contracts.Autenticacion;

public sealed record LoginResponse(
    string IdUsuario,
    string Nombre,
    string Correo,
    string TipoUsuario,
    string Token,
    DateTimeOffset ExpiraEn,
    decimal? Peso,
    decimal? Imc,
    int? CaloriasDiariasMax,
    string? Apellidos,
    int? Edad,
    string? Pais);

public sealed record UsuarioActualResponse(
    string IdUsuario,
    string Nombre,
    string Correo,
    string TipoUsuario);