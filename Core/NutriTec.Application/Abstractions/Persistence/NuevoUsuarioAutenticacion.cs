namespace NutriTec.Application.Abstractions.Persistence;

/*
 * Descripción:
 * Representa los datos mínimos para registrar un usuario desde el caso de uso de autenticación.
 *
 * Entradas:
 * Recibe nombre, correo, hash de contraseña y tipo de usuario preparados por Application.
 *
 * Salidas:
 * Transporta datos hacia el repositorio sin depender de una entidad o tabla concreta.
 *
 * Restricciones:
 * Debe recibir una contraseña ya hasheada; nunca debe transportar contraseñas en texto claro hacia persistencia.
 */
public sealed record NuevoUsuarioAutenticacion(
    string Nombre,
    string Correo,
    string ContrasenaHash,
    string TipoUsuario);
