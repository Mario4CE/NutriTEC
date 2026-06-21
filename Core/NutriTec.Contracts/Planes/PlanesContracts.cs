namespace NutriTec.Contracts.Planes;

public enum TiempoComida
{
    Desayuno = 1,
    MeriendaManana = 2,
    Almuerzo = 3,
    MeriendaTarde = 4,
    Cena = 5
}

public sealed record ItemPlanRequest(
    TiempoComida TiempoComida,
    Guid IdProducto,
    decimal Porciones);

public sealed record CrearPlanRequest(
    string Nombre,
    IReadOnlyCollection<ItemPlanRequest> Items);

public sealed record ActualizarPlanRequest(
    string Nombre,
    IReadOnlyCollection<ItemPlanRequest> Items);

public sealed record ItemPlanResponse(
    int Id,
    TiempoComida TiempoComida,
    Guid IdProducto,
    string NombreProducto,
    decimal Porciones,
    decimal CaloriasAportadas);

public sealed record PlanResponse(
    int Id,
    string IdNutricionista,
    string Nombre,
    DateTime FechaCreacionUtc,
    IReadOnlyCollection<ItemPlanResponse> Items,
    decimal CaloriasTotales);

public sealed record AsignarPlanRequest(
    int IdPaciente,
    int IdPlan,
    DateOnly FechaInicio,
    DateOnly FechaFin);

public sealed record AsignacionPlanResponse(
    int Id,
    int IdPaciente,
    int IdPlan,
    string IdNutricionista,
    DateOnly FechaInicio,
    DateOnly FechaFin,
    DateTime FechaAsignacionUtc);