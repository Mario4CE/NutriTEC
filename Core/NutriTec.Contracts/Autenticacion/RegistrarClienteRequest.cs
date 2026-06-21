using System.ComponentModel.DataAnnotations;

namespace NutriTec.Contracts.Autenticacion;

public sealed record RegistrarClienteRequest(
    [Required, MaxLength(100)] string Nombre,
    [Required, MaxLength(150)] string Apellidos,
    [Range(0, 130)] int Edad,
    DateOnly FechaNacimiento,
    [Range(0.01, 1000)] decimal Peso,
    [Range(0.01, 200)] decimal Imc,
    [Required, MaxLength(100)] string Pais,
    decimal? Cintura,
    decimal? Cuello,
    decimal? Caderas,
    decimal? PctMusculo,
    decimal? PctGrasa,
    [Range(1, 50000)] int CaloriasDiariasMax,
    [Required, EmailAddress, MaxLength(100)] string Correo,
    [Required, MinLength(8), MaxLength(100)] string Contrasena);
