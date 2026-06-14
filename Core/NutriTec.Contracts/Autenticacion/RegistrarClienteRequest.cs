using System.ComponentModel.DataAnnotations;

namespace NutriTec.Contracts.Autenticacion;

/*
 * Descripción:
 * Define los datos requeridos para registrar un cliente en NutriTEC conforme a la tabla SQL USUARIO.
 *
 * Entradas:
 * Recibe datos personales, métricas corporales, correo electrónico y contraseña inicial enviados por la API.
 *
 * Salidas:
 * Transporta la solicitud de registro hacia la capa de aplicación sin acoplarla a persistencia.
 *
 * Restricciones:
 * La contraseña debe ser hasheada antes de persistirse, el correo debe ser único en la fuente relacional y los campos opcionales de medidas corporales pueden omitirse.
 */

public sealed record RegistrarClienteRequest(
    [Required, MaxLength(100)] string Nombre,
    [Required, MaxLength(100)] string Apellidos,
    [Range(0, 130)] int Edad,
    DateOnly FechaNacimiento,
    [Range(typeof(decimal), "0.01", "999.99")] decimal Peso,
    [Range(typeof(decimal), "0.01", "999.99")] decimal Imc,
    [Required, MaxLength(100)] string Pais,
    [Range(typeof(decimal), "0", "999.99")] decimal? Cintura,
    [Range(typeof(decimal), "0", "999.99")] decimal? Cuello,
    [Range(typeof(decimal), "0", "999.99")] decimal? Caderas,
    [Range(typeof(decimal), "0", "100")] decimal? PctMusculo,
    [Range(typeof(decimal), "0", "100")] decimal? PctGrasa,
    [Range(typeof(decimal), "0", "99999.99")] decimal CaloriasDiariasMax,
    [Required, EmailAddress, MaxLength(254)] string Correo,
    [Required, MinLength(8), MaxLength(128)] string Contrasena);
