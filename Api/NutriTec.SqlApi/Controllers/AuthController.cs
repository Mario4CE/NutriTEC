using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using NutriTec.Application.Abstractions.Services;
using NutriTec.Contracts.Autenticacion;

namespace NutriTec.SqlApi.Controllers;

[ApiController]
[AllowAnonymous]
[Route("api/[controller]")]
public sealed class AuthController(IAuthService authService) : ControllerBase
{
    private const string CredencialesInvalidas = "Correo o contraseña inválidos.";

    /*
    Descripción:
    Autentica usuarios registrados sin revelar si falló el correo o la contraseña.

    Entradas:
    LoginRequest con correo y contraseña enviados por el cliente HTTP.

    Salidas:
    200 OK con LoginResponse si las credenciales son válidas; 401 Unauthorized con mensaje genérico si fallan.

    Restricciones:
    No accede a DbContext, no expone entidades SQL, no devuelve contraseñas ni password_hash; el JWT lo genera Application mediante la abstracción de tokens.
    */
    [HttpPost("login")]
    [EnableRateLimiting("login")]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        var response = await authService.LoginAsync(request, cancellationToken);

        if (response is null)
        {
            return Unauthorized(new { mensaje = CredencialesInvalidas });
        }

        return Ok(response);
    }

    /*
    Descripción:
    Registra un cliente usando el caso de uso de Application y devuelve únicamente el DTO público de autenticación.

    Entradas:
    RegistrarClienteRequest validado por DataAnnotations y reglas de negocio de Application.

    Salidas:
    201 Created con LoginResponse del cliente registrado.

    Restricciones:
    No accede a DbContext, no expone UsuarioSql y no devuelve contraseñas ni password_hash.
    */
    [HttpPost("registrar-cliente")]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<LoginResponse>> RegistrarCliente([FromBody] RegistrarClienteRequest request, CancellationToken cancellationToken)
    {
        var response = await authService.RegistrarClienteAsync(request, cancellationToken);

        return CreatedAtAction(nameof(Login), new { correo = response.Correo }, response);
    }

    /*
    Descripción:
    Registra un nutricionista usando el caso de uso de Application y mantiene el API desacoplado de SQL Server.

    Entradas:
    RegistrarNutricionistaRequest validado por DataAnnotations y reglas de negocio de Application.

    Salidas:
    201 Created con LoginResponse del nutricionista registrado.

    Restricciones:
    No accede a DbContext, no expone NutricionistaSql y no devuelve contraseñas ni password_hash.
    */
    [HttpPost("registrar-nutricionista")]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<LoginResponse>> RegistrarNutricionista([FromBody] RegistrarNutricionistaRequest request, CancellationToken cancellationToken)
    {
        var response = await authService.RegistrarNutricionistaAsync(request, cancellationToken);

        return CreatedAtAction(nameof(Login), new { correo = response.Correo }, response);
    }
}
