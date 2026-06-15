using System.Security.Cryptography;
using NutriTec.Application.Abstractions.Services;

namespace NutriTec.Infrastructure.Sql.Security;

public sealed class PasswordHasher : IPasswordHasher
{
    private const string Algoritmo = "PBKDF2-SHA256";
    private const int Iteraciones = 310000;
    private const int SaltBytes = 16;
    private const int HashBytes = 32;

    public string GenerarHash(string contrasena)
    {
        var salt = RandomNumberGenerator.GetBytes(SaltBytes);
        var hash = Rfc2898DeriveBytes.Pbkdf2(contrasena, salt, Iteraciones, HashAlgorithmName.SHA256, HashBytes);

        return $"{Algoritmo}${Iteraciones}${Convert.ToBase64String(salt)}${Convert.ToBase64String(hash)}";
    }

    public bool Verificar(string contrasena, string hashAlmacenado)
    {
        var partes = hashAlmacenado.Split('$');

        if (partes.Length != 4 || partes[0] != Algoritmo || !int.TryParse(partes[1], out var iteraciones))
        {
            return false;
        }

        var salt = Convert.FromBase64String(partes[2]);
        var hashEsperado = Convert.FromBase64String(partes[3]);
        var hashCalculado = Rfc2898DeriveBytes.Pbkdf2(contrasena, salt, iteraciones, HashAlgorithmName.SHA256, hashEsperado.Length);

        return CryptographicOperations.FixedTimeEquals(hashCalculado, hashEsperado);
    }
}
