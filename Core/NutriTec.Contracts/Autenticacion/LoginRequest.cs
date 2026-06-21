using System.ComponentModel.DataAnnotations;

namespace NutriTec.Contracts.Autenticacion;

public sealed record LoginRequest(
    [Required, EmailAddress, MaxLength(100)] string Correo,
    [Required, MinLength(8), MaxLength(100)] string Contrasena);
