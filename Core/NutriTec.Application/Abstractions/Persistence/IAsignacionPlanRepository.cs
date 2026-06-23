using NutriTec.Domain.Planes;

namespace NutriTec.Application.Abstractions.Persistence;

/*
 * Descripción:
 * Define las operaciones relacionales requeridas para asignar un plan de alimentación
 * a un paciente durante un periodo de tiempo.
 *
 * Entradas:
 * Recibe agregados, identificadores y criterios de búsqueda del dominio.
 *
 * Salidas:
 * Devuelve asignaciones persistidas o confirma modificaciones.
 *
 * Restricciones:
 * No expone Entity Framework Core para mantener Application desacoplada de SQL Server.
 */

public interface IAsignacionPlanRepository
{
    /*
     * Descripción: Persiste la asignación de un plan a un paciente.
     * Entradas: Agregado y cancelación.
     * Salidas: Agregado persistido.
     * Restricciones: Recibe datos validados.
     */

    Task<AsignacionPlan> AsignarAsync(AsignacionPlan asignacion, CancellationToken cancellationToken);

    /*
     * Descripción: Consulta la asignación de plan vigente para un paciente en la fecha actual.
     * Entradas: Identificador del paciente y cancelación.
     * Salidas: Asignación vigente o nula.
     * Restricciones: No modifica datos.
     */

    Task<AsignacionPlan?> ObtenerVigentePorPacienteAsync(int idPaciente, CancellationToken cancellationToken);

    /*
     * Descripción: Lista el historial de asignaciones de un paciente.
     * Entradas: Identificador del paciente y cancelación.
     * Salidas: Colección de asignaciones ordenadas por fecha.
     * Restricciones: No modifica datos.
     */

    Task<IReadOnlyCollection<AsignacionPlan>> ListarPorPacienteAsync(int idPaciente, CancellationToken cancellationToken);
}