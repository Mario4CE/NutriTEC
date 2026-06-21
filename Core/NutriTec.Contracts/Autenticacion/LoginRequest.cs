using System.ComponentModel.DataAnnotations;

namespace NutriTec.Contracts.Autenticacion;

public sealed class LoginRequest
{
    [Required, EmailAddress, MaxLength(100)]
    public string Correo { get; init; } = string.Empty;

    [Required, MinLength(8), MaxLength(100)]
    public string Contrasena { get; init; } = string.Empty;
}