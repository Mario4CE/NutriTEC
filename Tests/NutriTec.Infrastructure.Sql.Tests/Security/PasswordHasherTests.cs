using NutriTec.Infrastructure.Sql.Security;
using Xunit;

namespace NutriTec.Infrastructure.Sql.Tests.Security;

public sealed class PasswordHasherTests
{
    [Fact]
    public void GenerarHash_CuandoRecibeContrasena_RetornaFormatoPbkdf2Esperado()
    {
        var hasher = new PasswordHasher();
        const string contrasena = "Password123!";

        var hash = hasher.GenerarHash(contrasena);
        var partes = hash.Split('$');

        Assert.Equal(4, partes.Length);
        Assert.Equal("PBKDF2-SHA256", partes[0]);
        Assert.Equal("310000", partes[1]);
        Assert.Equal(16, Convert.FromBase64String(partes[2]).Length);
        Assert.Equal(32, Convert.FromBase64String(partes[3]).Length);
        Assert.DoesNotContain(contrasena, hash, StringComparison.Ordinal);
    }

    [Fact]
    public void GenerarHash_CuandoRecibeMismaContrasena_RetornaHashesDiferentes()
    {
        var hasher = new PasswordHasher();
        const string contrasena = "Password123!";

        var primerHash = hasher.GenerarHash(contrasena);
        var segundoHash = hasher.GenerarHash(contrasena);

        Assert.NotEqual(primerHash, segundoHash);
    }

    [Fact]
    public void Verificar_CuandoContrasenaEsCorrecta_RetornaTrue()
    {
        var hasher = new PasswordHasher();
        const string contrasena = "Password123!";
        var hash = hasher.GenerarHash(contrasena);

        var resultado = hasher.Verificar(contrasena, hash);

        Assert.True(resultado);
    }

    [Fact]
    public void Verificar_CuandoContrasenaEsIncorrecta_RetornaFalse()
    {
        var hasher = new PasswordHasher();
        var hash = hasher.GenerarHash("Password123!");

        var resultado = hasher.Verificar("OtraPassword123!", hash);

        Assert.False(resultado);
    }

    [Theory]
    [InlineData("")]
    [InlineData("hash-invalido")]
    [InlineData("PBKDF2-SHA512$310000$c2FsdA==$aGFzaA==")]
    [InlineData("PBKDF2-SHA256$iteraciones$c2FsdA==$aGFzaA==")]
    public void Verificar_CuandoHashTieneFormatoNoSoportado_RetornaFalse(string hashAlmacenado)
    {
        var hasher = new PasswordHasher();

        var resultado = hasher.Verificar("Password123!", hashAlmacenado);

        Assert.False(resultado);
    }
}
