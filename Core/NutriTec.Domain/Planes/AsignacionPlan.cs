namespace NutriTec.Domain.Planes;

/*
 * Descripción:
 * Representa la asignación de un plan de alimentación a un paciente durante un
 * periodo de tiempo determinado.
 *
 * Entradas:
 * Recibe el paciente, el plan asignado y las fechas de inicio y fin del periodo.
 *
 * Salidas:
 * Permite que el paciente conozca su consumo máximo de calorías por tiempo de comida
 * y total para su día durante el periodo vigente.
 *
 * Restricciones:
 * La fecha de inicio no puede ser posterior a la fecha de fin.
 */
public sealed class AsignacionPlan
{
    public Guid Id { get; init; }
    public Guid IdPaciente { get; init; }
    public Guid IdPlan { get; init; }
    public Guid IdNutricionista { get; init; }
    public DateOnly FechaInicio { get; init; }
    public DateOnly FechaFin { get; init; }
    public DateTime FechaAsignacionUtc { get; init; }
}