using NutriTec.Contracts.Retroalimentaciones;

namespace NutriTec.Application.Abstractions.Services;

/*
 * Descripción:
 * Define los casos de uso disponibles para gestionar foros de retroalimentación.
 *
 * Entradas:
 * Recibe DTOs e identificadores enviados por la API.
 *
 * Salidas:
 * Devuelve DTOs de respuesta y el resultado de las actualizaciones.
 *
 * Restricciones:
 * Implementa el límite entre controllers y persistencia; los controllers no acceden a repositorios.
 */
public interface IRetroalimentacionService
{
    /*
     * Descripción:
     * Abre un foro con su mensaje inicial.
     *
     * Entradas:
     * Recibe DTO de creación y token de cancelación.
     *
     * Salidas:
     * Devuelve el DTO del foro creado.
     *
     * Restricciones:
     * Debe validar participantes y contenido antes de persistir.
     */
    Task<RetroalimentacionResponse> CrearAsync(CrearRetroalimentacionRequest request, CancellationToken cancellationToken);

    /*
     * Descripción:
     * Consulta los foros visibles para un paciente.
     *
     * Entradas:
     * Recibe identificador del paciente y token de cancelación.
     *
     * Salidas:
     * Devuelve DTOs de retroalimentación.
     *
     * Restricciones:
     * No expone entidades de dominio.
     */
    Task<IReadOnlyCollection<RetroalimentacionResponse>> ObtenerPorPacienteAsync(Guid idPaciente, CancellationToken cancellationToken);

    /*
     * Descripción:
     * Consulta los foros visibles para un nutricionista.
     *
     * Entradas:
     * Recibe identificador del nutricionista y token de cancelación.
     *
     * Salidas:
     * Devuelve DTOs de retroalimentación.
     *
     * Restricciones:
     * No expone entidades de dominio.
     */
    Task<IReadOnlyCollection<RetroalimentacionResponse>> ObtenerPorNutricionistaAsync(Guid idNutricionista, CancellationToken cancellationToken);

    /*
     * Descripción:
     * Responde un foro existente.
     *
     * Entradas:
     * Recibe identificador del foro, DTO de respuesta y token de cancelación.
     *
     * Salidas:
     * Devuelve verdadero cuando se agregó el mensaje.
     *
     * Restricciones:
     * Debe validar autor y contenido antes de persistir.
     */
    Task<bool> ResponderAsync(Guid idRetroalimentacion, ResponderRetroalimentacionRequest request, CancellationToken cancellationToken);
}
