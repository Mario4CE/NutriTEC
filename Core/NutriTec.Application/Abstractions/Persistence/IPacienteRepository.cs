using NutriTec.Domain.Pacientes;

namespace NutriTec.Application.Abstractions.Persistence;

/*
 * Descripción:
 * Define las operaciones relacionales requeridas por el módulo de asociación entre
 * nutricionistas y sus pacientes.
 *
 * Entradas:
 * Recibe agregados, identificadores y criterios de búsqueda del dominio.
 *
 * Salidas:
 * Devuelve asociaciones persistidas o confirma modificaciones.
 *
 * Restricciones:
 * No expone Entity Framework Core para mantener Application desacoplada de SQL Server.
 */

public interface IPacienteRepository
{
    /*
     * Descripción: Persiste la asociación entre un nutricionista y un paciente.
     * Entradas: Agregado y cancelación.
     * Salidas: Agregado persistido.
     * Restricciones: Recibe datos validados.
     */

    Task<PacienteNutricionista> AsociarAsync(PacienteNutricionista asociacion, CancellationToken cancellationToken);

    /*
     * Descripción: Verifica si un cliente ya está asociado a un nutricionista.
     * Entradas: Cédula del nutricionista, identificador del cliente y cancelación.
     * Salidas: Existencia de la asociación.
     * Restricciones: No modifica datos.
     */

    Task<bool> ExisteAsociacionAsync(string idNutricionista, int idPaciente, CancellationToken cancellationToken);

    /*
     * Descripción: Lista los pacientes asociados a un nutricionista.
     * Entradas: Cédula del nutricionista y cancelación.
     * Salidas: Colección de asociaciones.
     * Restricciones: No modifica datos.
     */

    Task<IReadOnlyCollection<PacienteNutricionista>> ListarPorNutricionistaAsync(string idNutricionista, CancellationToken cancellationToken);

    /*
     * Descripción: Consulta una asociación por identificador.
     * Entradas: Identificador y cancelación.
     * Salidas: Asociación o nulo.
     * Restricciones: No modifica datos.
     */

    Task<PacienteNutricionista?> ObtenerPorIdAsync(Guid id, CancellationToken cancellationToken);

    /*
     * Descripción: Verifica si un paciente pertenece a un nutricionista específico.
     * Entradas: Cédula del nutricionista, identificador del paciente y cancelación.
     * Salidas: Verdadero si el paciente está asociado a ese nutricionista.
     * Restricciones: No modifica datos.
     */

    Task<bool> EsPacienteDeAsync(string idNutricionista, int idPaciente, CancellationToken cancellationToken);

    /*
     * Descripción: Elimina la asociación entre un nutricionista y un paciente.
     * Entradas: Identificador de la asociación y cancelación.
     * Salidas: Confirmación.
     * Restricciones: Devuelve falso si no existe.
     */

    Task<bool> DesasociarAsync(Guid id, CancellationToken cancellationToken);
}