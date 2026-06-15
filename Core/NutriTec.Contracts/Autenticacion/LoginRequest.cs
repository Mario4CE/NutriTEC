using System.ComponentModel.DataAnnotations;

namespace NutriTec.Contracts.Autenticacion;

public sealed record LoginRequest(
    [property: Required, EmailAddress, MaxLength(100)] string Correo,
    [property: Required, MinLength(8), MaxLength(100)] string Contrasena);
