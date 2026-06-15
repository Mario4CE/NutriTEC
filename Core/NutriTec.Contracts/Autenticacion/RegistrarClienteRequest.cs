using System.ComponentModel.DataAnnotations;

namespace NutriTec.Contracts.Autenticacion;

public sealed record RegistrarClienteRequest(
    [property: Required, MaxLength(100)] string Nombre,
    [property: Required, MaxLength(150)] string Apellidos,
    [property: Range(0, 130)] int Edad,
    DateOnly FechaNacimiento,
    [property: Range(0.01, 1000)] decimal Peso,
    [property: Range(0.01, 200)] decimal Imc,
    [property: Required, MaxLength(100)] string Pais,
    decimal? Cintura,
    decimal? Cuello,
    decimal? Caderas,
    decimal? PctMusculo,
    decimal? PctGrasa,
    [property: Range(1, 50000)] int CaloriasDiariasMax,
    [property: Required, EmailAddress, MaxLength(100)] string Correo,
    [property: Required, MinLength(8), MaxLength(100)] string Contrasena);
