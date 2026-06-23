using NutriTec.Contracts.Planes;

namespace NutriTec.Application.Abstractions.Services;

/*
 * Descripción:
 * Define las reglas de aplicación para que un nutricionista asigne un plan de
 * alimentación a un paciente durante un periodo de tiempo determinado.
 *
 * Entradas:
 * Recibe DTOs de solicitud provenientes de la API.
 *
 * Salidas:
 * Devuelve DTOs preparados para la respuesta de la API.
 *
 * Restricciones:
 * No expone entidades de dominio fuera de la capa de aplicación.
 */

public interface IAsignacionPlanService
{
    /*
     * Descripción: Asigna un plan a un paciente para un periodo de tiempo específico.
     * Entradas: Cédula del nutricionista, DTO de asignación y cancelación.
     * Salidas: Devuelve la asignación creada como DTO.
     * Restricciones: El paciente debe estar asociado al nutricionista y el plan debe pertenecerle.
     */

    Task<AsignacionPlanResponse> AsignarAsync(string idNutricionista, AsignarPlanRequest request, CancellationToken cancellationToken);

    /*
     * Descripción: Consulta la asignación de plan vigente para un paciente.
     * Entradas: Identificador del paciente y cancelación.
     * Salidas: Devuelve la asignación vigente como DTO, o nulo si no hay ninguna.
     * Restricciones: El identificador no puede ser vacío.
     */

    Task<AsignacionPlanResponse?> ObtenerVigentePorPacienteAsync(int idPaciente, CancellationToken cancellationToken);

    /*
     * Descripción: Lista el historial de asignaciones de un paciente.
     * Entradas: Identificador del paciente y cancelación.
     * Salidas: Devuelve una colección de DTOs ordenada por fecha.
     * Restricciones: El identificador no puede ser vacío.
     */

    Task<IReadOnlyCollection<AsignacionPlanResponse>> ListarPorPacienteAsync(int idPaciente, CancellationToken cancellationToken);
}