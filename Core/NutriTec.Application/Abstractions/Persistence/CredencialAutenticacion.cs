namespace NutriTec.Application.Abstractions.Persistence;

/*
 * Descripción:
 * Representa la proyección mínima de credenciales requerida por el caso de uso de login.
 *
 * Entradas:
 * Recibe datos seguros recuperados desde la persistencia relacional mediante una abstracción.
 *
 * Salidas:
 * Transporta información interna para validar credenciales y construir una respuesta de autenticación.
 *
 * Restricciones:
 * No debe exponerse al frontend porque contiene el hash de la contraseña persistida; el identificador se mantiene como texto para soportar claves SQL heterogéneas.
 */

public sealed record CredencialAutenticacion(
    string IdUsuario,
    string Nombre,
    string Correo,
    string ContrasenaHash,
    string TipoUsuario);
