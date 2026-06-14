namespace NutriTec.Application.Abstractions.Persistence;

/*
 * Descripción:
 * Representa los datos necesarios para registrar un usuario desde el caso de uso de autenticación conforme al esquema SQL actual.
 *
 * Entradas:
 * Recibe datos personales, datos específicos por tipo de usuario, correo, hash de contraseña y tipo de usuario preparados por Application.
 *
 * Salidas:
 * Transporta datos hacia el repositorio sin depender de una entidad o tabla concreta.
 *
 * Restricciones:
 * Debe recibir una contraseña ya hasheada; nunca debe transportar contraseñas en texto claro hacia persistencia. Los campos específicos se completan según el TipoUsuario.
 */

public sealed record NuevoUsuarioAutenticacion(
    string? Cedula,
    string Nombre,
    string Apellidos,
    int Edad,
    DateOnly FechaNacimiento,
    decimal Peso,
    decimal Imc,
    string? Pais,
    decimal? Cintura,
    decimal? Cuello,
    decimal? Caderas,
    decimal? PctMusculo,
    decimal? PctGrasa,
    decimal? CaloriasDiariasMax,
    string? CodigoNutricionista,
    string? Direccion,
    string? FotoUrl,
    string? TarjetaCredito,
    string? TipoCobro,
    string Correo,
    string ContrasenaHash,
    string TipoUsuario);
