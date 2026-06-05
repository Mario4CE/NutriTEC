namespace NutriTec.Contracts.Autenticacion;

/*
 * Descripción:
 * Expone la información segura que identifica a un usuario autenticado.
 *
 * Entradas:
 * Recibe identificador, nombre, correo y tipo de usuario desde el caso de uso de autenticación.
 *
 * Salidas:
 * Transporta datos serializables hacia el frontend después de validar credenciales.
 *
 * Restricciones:
 * No incluye contraseña, hash ni token JWT porque la emisión de tokens se implementará en un incremento posterior.
 */
public sealed record LoginResponse(
    Guid IdUsuario,
    string Nombre,
    string Correo,
    string TipoUsuario);
