namespace NutriTec.Application.Abstractions.Persistence;

public sealed record CredencialAutenticacion(
    string IdUsuario,
    string Nombre,
    string Correo,
    string ContrasenaHash,
    string TipoUsuario);
