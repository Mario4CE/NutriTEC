namespace NutriTec.Domain.Retroalimentaciones;

/*
 * Descripción:
 * Representa una participación dentro del foro de retroalimentación entre paciente y nutricionista.
 *
 * Entradas:
 * Recibe el autor, el contenido textual y la fecha UTC del mensaje.
 *
 * Salidas:
 * Conserva una entrada inmutable de la conversación.
 *
 * Restricciones:
 * El autor y el contenido no pueden estar vacíos; la validación se aplica desde la capa de aplicación.
 */
public sealed class MensajeRetroalimentacion
{
    public string Autor { get; init; } = string.Empty;
    public string Mensaje { get; init; } = string.Empty;
    public DateTime FechaUtc { get; init; }
}
