namespace NutriTec.Application.Abstractions.Persistence;

public sealed record CredencialAutenticacion(
    string IdUsuario,
    string Nombre,
    string Correo,
    string ContrasenaHash,
    string TipoUsuario,
    decimal? Peso,
    decimal? Imc,
    int? CaloriasDiariasMax,
    string? Apellidos,
    int? Edad,
    string? Pais);