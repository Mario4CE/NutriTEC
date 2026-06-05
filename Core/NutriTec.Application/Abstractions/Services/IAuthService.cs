using NutriTec.Contracts.Autenticacion;

namespace NutriTec.Application.Abstractions.Services;

/*
 * Descripción:
 * Define los casos de uso iniciales para autenticación y registro de usuarios.
 *
 * Entradas:
 * Recibe DTOs de login y registro enviados por la API SQL junto con tokens de cancelación.
 *
 * Salidas:
 * Devuelve información segura del usuario autenticado o registrado.
 *
 * Restricciones:
 * No emite JWT todavía y no debe persistir contraseñas sin aplicar hashing mediante una abstracción segura.
 */
public interface IAuthService
{
    /* Descripción: Valida credenciales de acceso. Entradas: DTO de login y cancelación. Salidas: Usuario autenticado o nulo. Restricciones: No devuelve secretos. */
    Task<LoginResponse?> LoginAsync(LoginRequest request, CancellationToken cancellationToken);

    /* Descripción: Registra un cliente. Entradas: DTO de registro y cancelación. Salidas: Usuario registrado. Restricciones: El correo debe ser único. */
    Task<LoginResponse> RegistrarClienteAsync(RegistrarClienteRequest request, CancellationToken cancellationToken);

    /* Descripción: Registra un nutricionista. Entradas: DTO de registro y cancelación. Salidas: Usuario registrado. Restricciones: El correo debe ser único. */
    Task<LoginResponse> RegistrarNutricionistaAsync(RegistrarNutricionistaRequest request, CancellationToken cancellationToken);
}
