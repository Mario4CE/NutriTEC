using System.ComponentModel.DataAnnotations;

namespace NutriTec.Contracts.Autenticacion;

/*
 * Descripción:
 * Define los datos mínimos para registrar un cliente en NutriTEC.
 *
 * Entradas:
 * Recibe nombre, correo electrónico y contraseña inicial enviados por la API.
 *
 * Salidas:
 * Transporta la solicitud de registro hacia la capa de aplicación sin acoplarla a persistencia.
 *
 * Restricciones:
 * La contraseña debe ser hasheada antes de persistirse y el correo debe ser único en la fuente relacional.
 */
public sealed record RegistrarClienteRequest(
    [Required, MaxLength(150)] string Nombre,
    [Required, EmailAddress, MaxLength(254)] string Correo,
    [Required, MinLength(8), MaxLength(128)] string Contrasena);
