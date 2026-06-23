namespace NutriTec.Infrastructure.Sql.Persistence.Entities;

public sealed class UsuarioSql
{
    public int IdUsuario { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Apellidos { get; set; } = string.Empty;
    public int Edad { get; set; }
    public DateOnly FechaNacimiento { get; set; }
    public decimal Peso { get; set; }
    public decimal Imc { get; set; }
    public string Pais { get; set; } = string.Empty;
    public decimal? Cintura { get; set; }
    public decimal? Cuello { get; set; }
    public decimal? Caderas { get; set; }
    public decimal? PctMusculo { get; set; }
    public decimal? PctGrasa { get; set; }
    public decimal CaloriasDiariasMax { get; set; }
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
}