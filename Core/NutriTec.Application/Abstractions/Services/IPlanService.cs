using NutriTec.Contracts.Planes;

namespace NutriTec.Application.Abstractions.Services;

/*
 * Descripción:
 * Define las reglas de aplicación para que un nutricionista gestione planes de
 * alimentación compuestos por productos distribuidos en cinco tiempos de comida.
 *
 * Entradas:
 * Recibe DTOs de solicitud provenientes de la API.
 *
 * Salidas:
 * Devuelve DTOs preparados para la respuesta de la API, incluyendo el total calórico.
 *
 * Restricciones:
 * No expone entidades de dominio fuera de la capa de aplicación.
 */

public interface IPlanService
{
    /*
     * Descripción: Crea un plan de alimentación para un nutricionista.
     * Entradas: Cédula del nutricionista, DTO de creación y cancelación.
     * Salidas: Devuelve el plan creado como DTO, con el total calórico calculado.
     * Restricciones: El nombre y al menos un item son obligatorios; los productos deben existir.
     */

    Task<PlanResponse> CrearAsync(string idNutricionista, CrearPlanRequest request, CancellationToken cancellationToken);

    /*
     * Descripción: Consulta un plan por identificador.
     * Entradas: Identificador del plan y cancelación.
     * Salidas: Devuelve el DTO encontrado o nulo.
     * Restricciones: El identificador no puede ser vacío.
     */

    Task<PlanResponse?> ObtenerPorIdAsync(int idPlan, CancellationToken cancellationToken);

    /*
     * Descripción: Lista los planes creados por un nutricionista.
     * Entradas: Cédula del nutricionista y cancelación.
     * Salidas: Devuelve una colección de DTOs.
     * Restricciones: La cédula no puede estar vacía.
     */

    Task<IReadOnlyCollection<PlanResponse>> ListarPorNutricionistaAsync(string idNutricionista, CancellationToken cancellationToken);

    /*
     * Descripción: Actualiza el nombre y los items de un plan existente.
     * Entradas: Cédula del nutricionista, identificador del plan, DTO de edición y cancelación.
     * Salidas: Devuelve verdadero cuando el plan existe, pertenece al nutricionista y fue actualizado.
     * Restricciones: El plan debe pertenecer al nutricionista que solicita la edición.
     */

    Task<bool> ActualizarAsync(string idNutricionista, int idPlan, ActualizarPlanRequest request, CancellationToken cancellationToken);

    /*
     * Descripción: Elimina un plan de alimentación.
     * Entradas: Cédula del nutricionista, identificador del plan y cancelación.
     * Salidas: Devuelve verdadero cuando el plan existe, pertenece al nutricionista y fue eliminado.
     * Restricciones: El plan debe pertenecer al nutricionista que solicita la eliminación.
     */

    Task<bool> EliminarAsync(string idNutricionista, int idPlan, CancellationToken cancellationToken);
}