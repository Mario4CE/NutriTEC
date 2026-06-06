namespace NutriTec.Contracts.Retroalimentaciones;

/*
 * Descripción:
 * Define los datos requeridos para agregar una respuesta a un foro existente.
 *
 * Entradas:
 * Recibe el autor y el contenido de la nueva respuesta.
 *
 * Salidas:
 * Transporta la respuesta desde la API hacia la capa de aplicación.
 *
 * Restricciones:
 * La capa de aplicación rechaza valores vacíos antes de actualizar el documento.
 */

public sealed record ResponderRetroalimentacionRequest(string Autor, string Mensaje);
