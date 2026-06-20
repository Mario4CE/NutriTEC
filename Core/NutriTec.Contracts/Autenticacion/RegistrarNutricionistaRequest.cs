using System.ComponentModel.DataAnnotations;

namespace NutriTec.Contracts.Autenticacion;

public sealed record RegistrarNutricionistaRequest(
    [property: Required, MaxLength(20)] string Cedula,
    [property: Required, MaxLength(100)] string Nombre,
    [property: Required, MaxLength(150)] string Apellidos,
    [property: Required, MaxLength(50)] string CodigoNutricionista,
    [property: Range(0, 130)] int Edad,
    DateOnly FechaNacimiento,
    [property: Range(0.01, 1000)] decimal Peso,
    [property: Range(0.01, 200)] decimal Imc,
    [property: Required, MaxLength(255)] string Direccion,
    [property: MaxLength(500)] string? FotoUrl,
    [property: Required, MaxLength(100)] string TarjetaCredito,
    [property: Required, RegularExpression("^(semanal|mensual|anual)$")] string TipoCobro,
    [property: Required, EmailAddress, MaxLength(100)] string Correo,
    [property: Required, MinLength(8), MaxLength(100)] string Contrasena);
