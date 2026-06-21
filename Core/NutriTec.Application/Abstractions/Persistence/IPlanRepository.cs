using NutriTec.Domain.Planes;

namespace NutriTec.Application.Abstractions.Persistence;

/*
 * Descripción:
 * Define las operaciones relacionales requeridas por el módulo de planes de alimentación.
 *
 * Entradas:
 * Recibe agregados, identificadores y criterios de búsqueda del dominio.
 *
 * Salidas:
 * Devuelve planes persistidos o confirma modificaciones.
 *
 * Restricciones:
 * No expone Entity Framework Core para mantener Application desacoplada de SQL Server.
 */

public interface IPlanRepository
{
    /*
     * Descripción: Persiste un plan con sus items.
     * Entradas: Agregado y cancelación.
     * Salidas: Agregado persistido, con el identificador generado por la base de datos.
     * Restricciones: Recibe datos validados.
     */

    Task<Plan> CrearAsync(Plan plan, CancellationToken cancellationToken);

    /*
     * Descripción: Consulta un plan por identificador, incluyendo sus items.
     * Entradas: Identificador y cancelación.
     * Salidas: Plan o nulo.
     * Restricciones: No modifica datos.
     */

    Task<Plan?> ObtenerPorIdAsync(int idPlan, CancellationToken cancellationToken);

    /*
     * Descripción: Lista los planes creados por un nutricionista.
     * Entradas: Cédula del nutricionista y cancelación.
     * Salidas: Colección de planes.
     * Restricciones: No modifica datos.
     */

    Task<IReadOnlyCollection<Plan>> ListarPorNutricionistaAsync(string idNutricionista, CancellationToken cancellationToken);

    /*
     * Descripción: Reemplaza el nombre y los items de un plan existente.
     * Entradas: Agregado con los datos actualizados y cancelación.
     * Salidas: Confirmación.
     * Restricciones: Recibe reglas ya aplicadas; reemplaza la colección completa de items.
     */

    Task<bool> ActualizarAsync(Plan plan, CancellationToken cancellationToken);

    /*
     * Descripción: Elimina un plan por identificador.
     * Entradas: Identificador y cancelación.
     * Salidas: Confirmación.
     * Restricciones: Devuelve falso si no existe.
     */

    Task<bool> EliminarAsync(int idPlan, CancellationToken cancellationToken);

    /*
     * Descripción: Verifica que un plan pertenezca a un nutricionista específico.
     * Entradas: Identificador del plan, cédula del nutricionista y cancelación.
     * Salidas: Verdadero si el plan existe y pertenece a ese nutricionista.
     * Restricciones: No modifica datos.
     */

    Task<bool> PertenecePlanAAsync(int idPlan, string idNutricionista, CancellationToken cancellationToken);
}