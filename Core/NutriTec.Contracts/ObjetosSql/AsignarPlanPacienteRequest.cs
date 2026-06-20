using System.ComponentModel.DataAnnotations;

namespace NutriTec.Contracts.ObjetosSql;

public sealed class AsignarPlanPacienteRequest
{
    [Required]
    public DateOnly FechaInicio { get; init; }

    [Required]
    public DateOnly FechaFin { get; init; }
}
