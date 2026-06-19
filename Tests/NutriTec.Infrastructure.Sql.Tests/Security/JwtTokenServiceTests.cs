using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Extensions.Options;
using NutriTec.Application.Autenticacion;
using NutriTec.Infrastructure.Sql.Security;
using Xunit;

namespace NutriTec.Infrastructure.Sql.Tests.Security;

public sealed class JwtTokenServiceTests
{
    [Fact]
    public void GenerarToken_CuandoConfiguracionEsValida_RetornaJwtConIssuerAudienceExpiracionYClaims()
    {
        var options = CrearJwtOptions(expirationMinutes: 45);
        var service = new JwtTokenService(Options.Create(options));
        var usuario = new UsuarioTokenAutenticacion(
            IdUsuario: "123",
            Nombre: "Cliente Demo",
            Correo: "cliente@example.com",
            TipoUsuario: "Cliente");
        var antes = DateTimeOffset.UtcNow;

        var token = service.GenerarToken(usuario);
        var despues = DateTimeOffset.UtcNow;
        var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token.Token);

        Assert.False(string.IsNullOrWhiteSpace(token.Token));
        Assert.Equal(options.Issuer, jwt.Issuer);
        Assert.Contains(options.Audience, jwt.Audiences);
        Assert.InRange(token.ExpiraEn, antes.AddMinutes(45).AddSeconds(-5), despues.AddMinutes(45).AddSeconds(5));
        Assert.Contains(jwt.Claims, claim => claim.Type == JwtRegisteredClaimNames.Sub && claim.Value == "123");
        Assert.Contains(jwt.Claims, claim => (claim.Type == JwtRegisteredClaimNames.Email || claim.Type == ClaimTypes.Email) && claim.Value == "cliente@example.com");
        Assert.Contains(jwt.Claims, claim => (claim.Type == JwtRegisteredClaimNames.Name || claim.Type == ClaimTypes.Name) && claim.Value == "Cliente Demo");
        Assert.Contains(jwt.Claims, claim => (claim.Type == JwtRegisteredClaimNames.Sub || claim.Type == ClaimTypes.NameIdentifier) && claim.Value == "123");
        Assert.Contains(jwt.Claims, claim => (claim.Type == "role" || claim.Type == ClaimTypes.Role) && claim.Value == "Cliente");
    }

    [Theory]
    [InlineData("", "NutriTec.Clients", "DevelopmentOnlyJwtSecretValueForTests12345", 60)]
    [InlineData("NutriTec.SqlApi", "", "DevelopmentOnlyJwtSecretValueForTests12345", 60)]
    [InlineData("NutriTec.SqlApi", "NutriTec.Clients", "", 60)]
    [InlineData("NutriTec.SqlApi", "NutriTec.Clients", "DevelopmentOnlyJwtSecretValueForTests12345", 0)]
    public void GenerarToken_CuandoConfiguracionEstaIncompleta_LanzaInvalidOperationException(
        string issuer,
        string audience,
        string secret,
        int expirationMinutes)
    {
        var service = new JwtTokenService(Options.Create(new JwtOptions
        {
            Issuer = issuer,
            Audience = audience,
            Secret = secret,
            ExpirationMinutes = expirationMinutes
        }));
        var usuario = new UsuarioTokenAutenticacion("123", "Cliente Demo", "cliente@example.com", "Cliente");

        var exception = Assert.Throws<InvalidOperationException>(() => service.GenerarToken(usuario));

        Assert.Equal("La configuración JWT está incompleta.", exception.Message);
    }

    private static JwtOptions CrearJwtOptions(int expirationMinutes) => new()
    {
        Issuer = "NutriTec.SqlApi",
        Audience = "NutriTec.Clients",
        Secret = "DevelopmentOnlyJwtSecretValueForTests12345",
        ExpirationMinutes = expirationMinutes
    };
}
