namespace NutriTec.Contracts.Autenticacion;

public sealed record LoginResponse(
    string IdUsuario,
    string Nombre,
    string Correo,
    string TipoUsuario,
    string Token,
    DateTimeOffset ExpiraEn);
