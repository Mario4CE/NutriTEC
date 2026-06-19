namespace NutriTec.Infrastructure.Sql.Persistence.Entities;

public sealed class PacienteNutricionistaSql
{
    public Guid Id { get; set; }
    public string IdNutricionista { get; set; } = string.Empty;
    public Guid IdPaciente { get; set; }
    public DateTime FechaAsociacionUtc { get; set; }
}