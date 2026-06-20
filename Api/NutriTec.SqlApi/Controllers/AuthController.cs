using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using NutriTec.Application.Abstractions.Services;
using NutriTec.Contracts.Autenticacion;
using NutriTec.Contracts.Common;

namespace NutriTec.SqlApi.Controllers;

[ApiController]
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
    [AllowAnonymous]
    [EnableRateLimiting("login")]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        var response = await authService.LoginAsync(request, cancellationToken);

        if (response is null)
        {
            return Unauthorized(new ErrorResponse("credenciales_invalidas", CredencialesInvalidas));
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
    No accede a DbContext, no expone UsuarioSql, no devuelve contraseñas, no devuelve password_hash y no crea JWT.
    */
    [HttpPost("registrar-cliente")]
    [AllowAnonymous]
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
    No accede a DbContext, no expone NutricionistaSql, no devuelve contraseñas, no devuelve password_hash y no crea JWT.
    */
    [HttpPost("registrar-nutricionista")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<LoginResponse>> RegistrarNutricionista([FromBody] RegistrarNutricionistaRequest request, CancellationToken cancellationToken)
    {
        var response = await authService.RegistrarNutricionistaAsync(request, cancellationToken);

        return CreatedAtAction(nameof(Login), new { correo = response.Correo }, response);
    }

    /*
    Descripción:
    Devuelve la identidad del usuario autenticado a partir de los claims del JWT validado.

    Entradas:
    Token JWT enviado en el encabezado Authorization como Bearer token.

    Salidas:
    200 OK con UsuarioActualResponse si el token es válido; 401 Unauthorized si faltan claims obligatorios.

    Restricciones:
    No consulta base de datos, no expone entidades SQL, no devuelve contraseñas ni password_hash y no emite nuevos tokens.
    */
    [HttpGet("me")]
    [Authorize]
    [ProducesResponseType(typeof(UsuarioActualResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    public ActionResult<UsuarioActualResponse> ObtenerUsuarioActual()
    {
        var idUsuario = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
        var nombre = User.FindFirstValue(ClaimTypes.Name) ?? User.FindFirstValue("name");
        var correo = User.FindFirstValue(ClaimTypes.Email) ?? User.FindFirstValue("email");
        var tipoUsuario = User.FindFirstValue(ClaimTypes.Role) ?? User.FindFirstValue("role");

        if (string.IsNullOrWhiteSpace(idUsuario)
            || string.IsNullOrWhiteSpace(nombre)
            || string.IsNullOrWhiteSpace(correo)
            || string.IsNullOrWhiteSpace(tipoUsuario))
        {
            return Unauthorized(new ErrorResponse("token_invalido", "El token de autenticación no es válido."));
        }

        return Ok(new UsuarioActualResponse(idUsuario, nombre, correo, tipoUsuario));
    }

}
