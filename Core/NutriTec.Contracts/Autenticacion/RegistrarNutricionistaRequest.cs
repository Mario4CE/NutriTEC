using System.ComponentModel.DataAnnotations;

namespace NutriTec.Contracts.Autenticacion;

/*
 * Descripción:
 * Define los datos mínimos para registrar un nutricionista en NutriTEC.
 *
 * Entradas:
 * Recibe nombre, correo electrónico y contraseña inicial enviados por la API.
 *
 * Salidas:
 * Transporta la solicitud de registro hacia la capa de aplicación sin exponer entidades relacionales.
 *
 * Restricciones:
 * La contraseña debe ser hasheada antes de persistirse y los datos profesionales específicos se agregarán cuando el dominio los defina.
 */

public sealed record RegistrarNutricionistaRequest(
    [Required, MaxLength(150)] string Nombre,
    [Required, EmailAddress, MaxLength(254)] string Correo,
    [Required, MinLength(8), MaxLength(128)] string Contrasena);
