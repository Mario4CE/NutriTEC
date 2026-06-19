namespace NutriTec.Contracts.Planes;

/*
 * Descripción:
 * Enumera los cinco tiempos de comida expuestos por la API, replicando el enum de Domain
 * sin crear una dependencia de Contracts hacia Domain.
 */
public enum TiempoComida
{
    Desayuno = 1,
    MeriendaManana = 2,
    Almuerzo = 3,
    MeriendaTarde = 4,
    Cena = 5
}

/*
 * Descripción:
 * DTO de un item dentro de la solicitud de creación o actualización de un plan.
 *
 * Entradas:
 * Recibe el tiempo de comida, el producto y la cantidad de porciones.
 *
 * Salidas:
 * No aplica.
 *
 * Restricciones:
 * Las porciones deben ser mayores que cero.
 */
public sealed record ItemPlanRequest(
    TiempoComida TiempoComida,
    Guid IdProducto,
    decimal Porciones);

/*
 * Descripción:
 * DTO de solicitud para crear un plan de alimentación.
 *
 * Entradas:
 * Recibe el nombre del plan y la lista de items distribuidos por tiempo de comida.
 *
 * Salidas:
 * No aplica.
 *
 * Restricciones:
 * El nombre y al menos un item son obligatorios.
 */
public sealed record CrearPlanRequest(
    string Nombre,
    IReadOnlyCollection<ItemPlanRequest> Items);

/*
 * Descripción:
 * DTO de solicitud para actualizar un plan de alimentación existente.
 *
 * Entradas:
 * Recibe el nombre del plan y la lista completa de items que reemplaza a la anterior.
 *
 * Salidas:
 * No aplica.
 *
 * Restricciones:
 * El nombre y al menos un item son obligatorios.
 */
public sealed record ActualizarPlanRequest(
    string Nombre,
    IReadOnlyCollection<ItemPlanRequest> Items);

/*
 * Descripción:
 * DTO de salida de un item de plan, incluyendo el detalle nutricional calculado.
 *
 * Entradas:
 * No aplica.
 *
 * Salidas:
 * Expone el tiempo de comida, el producto, las porciones y las calorías aportadas.
 *
 * Restricciones:
 * No aplica.
 */
public sealed record ItemPlanResponse(
    Guid Id,
    TiempoComida TiempoComida,
    Guid IdProducto,
    string NombreProducto,
    decimal Porciones,
    decimal CaloriasAportadas);

/*
 * Descripción:
 * DTO de salida que representa un plan de alimentación completo con su total calórico.
 *
 * Entradas:
 * No aplica.
 *
 * Salidas:
 * Expone el plan, sus items por tiempo de comida y el total de calorías calculado.
 *
 * Restricciones:
 * No aplica.
 */
public sealed record PlanResponse(
    Guid Id,
    string IdNutricionista,
    string Nombre,
    DateTime FechaCreacionUtc,
    IReadOnlyCollection<ItemPlanResponse> Items,
    decimal CaloriasTotales);

/*
 * Descripción:
 * DTO de solicitud para asignar un plan de alimentación a un paciente.
 *
 * Entradas:
 * Recibe el identificador del paciente, el plan y el periodo de vigencia.
 *
 * Salidas:
 * No aplica.
 *
 * Restricciones:
 * La fecha de inicio no puede ser posterior a la fecha de fin.
 */
public sealed record AsignarPlanRequest(
    Guid IdPaciente,
    Guid IdPlan,
    DateOnly FechaInicio,
    DateOnly FechaFin);

/*
 * Descripción:
 * DTO de salida que representa la asignación vigente o histórica de un plan a un paciente.
 *
 * Entradas:
 * No aplica.
 *
 * Salidas:
 * Expone el paciente, el plan, el periodo y la fecha en que se realizó la asignación.
 *
 * Restricciones:
 * No aplica.
 */
public sealed record AsignacionPlanResponse(
    Guid Id,
    Guid IdPaciente,
    Guid IdPlan,
    string IdNutricionista,
    DateOnly FechaInicio,
    DateOnly FechaFin,
    DateTime FechaAsignacionUtc);