namespace NutriTec.Infrastructure.Sql.Persistence.Entities;

public sealed class NutricionistaSql
{
    public string Cedula { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string Apellidos { get; set; } = string.Empty;
    public string CodigoNutricionista { get; set; } = string.Empty;
    public int Edad { get; set; }
    public DateOnly FechaNacimiento { get; set; }
    public decimal Peso { get; set; }
    public decimal Imc { get; set; }
    public string Direccion { get; set; } = string.Empty;
    public string? FotoUrl { get; set; }
    public string TarjetaCredito { get; set; } = string.Empty;
    public string TipoCobro { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
}

public sealed class TipoCobroSql
{
    public string CodigoTipoCobro { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public bool Activo { get; set; } = true;
}
