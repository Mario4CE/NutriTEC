using System.ComponentModel.DataAnnotations;

namespace NutriTec.Contracts.Autenticacion;

/*
 * Descripción:
 * Define las credenciales requeridas para iniciar sesión en NutriTEC.
 *
 * Entradas:
 * Recibe correo electrónico y contraseña en texto claro únicamente durante la solicitud HTTP.
 *
 * Salidas:
 * Transporta las credenciales desde la API hacia la capa de aplicación sin exponer entidades internas.
 *
 * Restricciones:
 * El correo debe tener formato válido y la contraseña no debe persistirse ni devolverse en respuestas.
 */
public sealed record LoginRequest(
    [Required, EmailAddress, MaxLength(254)] string Correo,
    [Required, MinLength(8), MaxLength(128)] string Contrasena);
