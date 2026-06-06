namespace NutriTec.Contracts.Retroalimentaciones;

/*
 * Descripción:
 * Expone un mensaje de retroalimentación sin revelar el modelo interno del dominio.
 *
 * Entradas:
 * Recibe autor, contenido y fecha UTC desde la capa de aplicación.
 *
 * Salidas:
 * Transporta un mensaje serializable hacia el frontend.
 *
 * Restricciones:
 * Solo se utiliza como DTO de salida.
 */

public sealed record MensajeRetroalimentacionResponse(string Autor, string Mensaje, DateTime FechaUtc);
