using System.ComponentModel.DataAnnotations;

namespace NutriTec.Contracts.Autenticacion;

public sealed record RegistrarNutricionistaRequest(
    [Required, MaxLength(20)] string Cedula,
    [Required, MaxLength(100)] string Nombre,
    [Required, MaxLength(150)] string Apellidos,
    [Required, MaxLength(50)] string CodigoNutricionista,
    [Range(0, 130)] int Edad,
    DateOnly FechaNacimiento,
    [Range(0.01, 1000)] decimal Peso,
    [Range(0.01, 200)] decimal Imc,
    [Required, MaxLength(255)] string Direccion,
    [MaxLength(500)] string? FotoUrl,
    [Required, MaxLength(100)] string TarjetaCredito,
    [Required, RegularExpression("^(semanal|mensual|anual)$")] string TipoCobro,
    [Required, EmailAddress, MaxLength(100)] string Correo,
    [Required, MinLength(8), MaxLength(100)] string Contrasena);
