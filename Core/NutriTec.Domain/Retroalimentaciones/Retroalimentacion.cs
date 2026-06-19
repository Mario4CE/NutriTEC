namespace NutriTec.Domain.Retroalimentaciones;

/*
 * Descripción:
 * Representa un foro documental de retroalimentación asociado con un paciente y un nutricionista.
 *
 * Entradas:
 * Recibe identificadores de participantes, fecha UTC de creación y mensajes ordenados cronológicamente.
 *
 * Salidas:
 * Mantiene el agregado que será persistido en MongoDB.
 *
 * Restricciones:
 * Debe contener participantes válidos y al menos el mensaje inicial al momento de su creación.
 */

public sealed class Retroalimentacion
{
    public Guid Id { get; init; }
    public Guid IdPaciente { get; init; }
    public Guid IdNutricionista { get; init; }
    public DateTime FechaCreacionUtc { get; init; }
    public List<MensajeRetroalimentacion> Mensajes { get; init; } = [];
}
