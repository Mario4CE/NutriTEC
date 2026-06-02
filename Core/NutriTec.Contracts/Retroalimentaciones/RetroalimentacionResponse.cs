namespace NutriTec.Contracts.Retroalimentaciones;

/*
 * Descripción:
 * Expone el estado completo de un foro de retroalimentación mediante un DTO independiente del dominio.
 *
 * Entradas:
 * Recibe identificadores, fecha UTC de creación y mensajes transformados por la capa de aplicación.
 *
 * Salidas:
 * Transporta una retroalimentación serializable hacia el frontend.
 *
 * Restricciones:
 * No debe contener tipos dependientes de MongoDB ni entidades internas.
 */
public sealed record RetroalimentacionResponse(
    Guid Id,
    Guid IdPaciente,
    Guid IdNutricionista,
    DateTime FechaCreacionUtc,
    IReadOnlyCollection<MensajeRetroalimentacionResponse> Mensajes);
