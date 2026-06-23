namespace NutriTec.Domain.Pacientes;

public sealed class PacienteNutricionista
{
    public Guid Id { get; init; }
    public string IdNutricionista { get; init; } = string.Empty;
    public int IdPaciente { get; init; }
    public DateTime FechaAsociacionUtc { get; init; }
}