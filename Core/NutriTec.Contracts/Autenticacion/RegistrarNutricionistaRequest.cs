using System.ComponentModel.DataAnnotations;

namespace NutriTec.Contracts.Autenticacion;

public sealed class RegistrarNutricionistaRequest
{
    [Required, MaxLength(20)]
    public string Cedula { get; init; } = string.Empty;

    [Required, MaxLength(100)]
    public string Nombre { get; init; } = string.Empty;

    [Required, MaxLength(150)]
    public string Apellidos { get; init; } = string.Empty;

    [Required, MaxLength(50)]
    public string CodigoNutricionista { get; init; } = string.Empty;

    [Range(0, 130)]
    public int Edad { get; init; }

    public DateOnly FechaNacimiento { get; init; }

    [Range(0.01, 1000)]
    public decimal Peso { get; init; }

    [Range(0.01, 200)]
    public decimal Imc { get; init; }

    [Required, MaxLength(255)]
    public string Direccion { get; init; } = string.Empty;

    [MaxLength(500)]
    public string? FotoUrl { get; init; }

    [Required, MaxLength(100)]
    public string TarjetaCredito { get; init; } = string.Empty;

    [Required, RegularExpression("^(semanal|mensual|anual)$")]
    public string TipoCobro { get; init; } = string.Empty;

    [Required, EmailAddress, MaxLength(100)]
    public string Correo { get; init; } = string.Empty;

    [Required, MinLength(8), MaxLength(100)]
    public string Contrasena { get; init; } = string.Empty;
}