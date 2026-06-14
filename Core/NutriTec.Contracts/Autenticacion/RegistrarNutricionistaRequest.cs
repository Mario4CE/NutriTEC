using System.ComponentModel.DataAnnotations;

namespace NutriTec.Contracts.Autenticacion;

/*
 * Descripción:
 * Define los datos requeridos para registrar un nutricionista en NutriTEC conforme a la tabla SQL NUTRICIONISTA.
 *
 * Entradas:
 * Recibe identificación, datos personales, datos profesionales, correo electrónico y contraseña inicial enviados por la API.
 *
 * Salidas:
 * Transporta la solicitud de registro hacia la capa de aplicación sin exponer entidades relacionales.
 *
 * Restricciones:
 * La contraseña debe ser hasheada antes de persistirse, el correo debe ser único y tipo_cobro debe corresponder a los valores aceptados por el esquema SQL.
 */

public sealed record RegistrarNutricionistaRequest(
    [Required, MaxLength(20)] string Cedula,
    [Required, MaxLength(100)] string Nombre,
    [Required, MaxLength(100)] string Apellidos,
    [Required, MaxLength(50)] string CodigoNutricionista,
    [Range(0, 130)] int Edad,
    DateOnly FechaNacimiento,
    [Range(typeof(decimal), "0.01", "999.99")] decimal Peso,
    [Range(typeof(decimal), "0.01", "999.99")] decimal Imc,
    [Required, MaxLength(255)] string Direccion,
    [MaxLength(500)] string? FotoUrl,
    [Required, MaxLength(20)] string TarjetaCredito,
    [Required, RegularExpression("^(semanal|mensual|anual)$"), MaxLength(10)] string TipoCobro,
    [Required, EmailAddress, MaxLength(254)] string Correo,
    [Required, MinLength(8), MaxLength(128)] string Contrasena);
