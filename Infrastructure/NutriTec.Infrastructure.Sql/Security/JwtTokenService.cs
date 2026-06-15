using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using NutriTec.Application.Abstractions.Services;
using NutriTec.Application.Autenticacion;

namespace NutriTec.Infrastructure.Sql.Security;

public sealed class JwtTokenService(IOptions<JwtOptions> jwtOptions) : ITokenService
{
    private readonly JwtOptions options = jwtOptions.Value;

    /*
    Descripción:
    Genera tokens JWT firmados para usuarios autenticados por Application.

    Entradas:
    UsuarioTokenAutenticacion con identificador, nombre, correo y tipo de usuario ya validados.

    Salidas:
    TokenAutenticacion con el JWT serializado y su fecha/hora de expiración.

    Restricciones:
    No valida contraseñas, no consulta base de datos, no guarda secretos y no incluye datos sensibles en claims.
    */
    public TokenAutenticacion GenerarToken(UsuarioTokenAutenticacion usuario)
    {
        ValidarConfiguracion();

        var ahora = DateTimeOffset.UtcNow;
        var expiraEn = ahora.AddMinutes(options.ExpirationMinutes);
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.Secret));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, usuario.IdUsuario),
            new Claim(JwtRegisteredClaimNames.Email, usuario.Correo),
            new Claim(ClaimTypes.NameIdentifier, usuario.IdUsuario),
            new Claim(JwtRegisteredClaimNames.Name, usuario.Nombre),
            new Claim(ClaimTypes.Name, usuario.Nombre),
            new Claim(ClaimTypes.Email, usuario.Correo),
            new Claim(ClaimTypes.Role, usuario.TipoUsuario),
            new Claim("role", usuario.TipoUsuario)
        };

        var token = new JwtSecurityToken(
            issuer: options.Issuer,
            audience: options.Audience,
            claims: claims,
            notBefore: ahora.UtcDateTime,
            expires: expiraEn.UtcDateTime,
            signingCredentials: credentials);

        return new TokenAutenticacion(new JwtSecurityTokenHandler().WriteToken(token), expiraEn);
    }

    private void ValidarConfiguracion()
    {
        if (string.IsNullOrWhiteSpace(options.Issuer)
            || string.IsNullOrWhiteSpace(options.Audience)
            || string.IsNullOrWhiteSpace(options.Secret)
            || options.ExpirationMinutes <= 0)
        {
            throw new InvalidOperationException("La configuración JWT está incompleta.");
        }
    }
}
