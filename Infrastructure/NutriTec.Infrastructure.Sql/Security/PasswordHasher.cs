using System.Security.Cryptography;
using NutriTec.Application.Abstractions.Services;

namespace NutriTec.Infrastructure.Sql.Security;

/*
 * Descripción:
 * Implementa el hashing seguro de contraseñas para la infraestructura SQL de NutriTEC.
 *
 * Entradas:
 * Recibe contraseñas en texto claro solo durante operaciones transitorias y hashes persistidos para verificación.
 *
 * Salidas:
 * Devuelve hashes con algoritmo, iteraciones, sal y clave derivada codificados para persistencia segura.
 *
 * Restricciones:
 * No guarda contraseñas en texto plano, no utiliza secretos reales y debe ejecutarse únicamente detrás de IPasswordHasher.
 */

public sealed class PasswordHasher : IPasswordHasher
{
    private const int SaltSizeBytes = 16;
    private const int HashSizeBytes = 32;
    private const int Iterations = 310_000;
    private const char Separator = '$';
    private const string Algorithm = "PBKDF2-SHA256";


    /*
     * Descripción:
     * Genera un hash persistible para una contraseña recibida en texto claro.
     * Entradas:
     * Recibe la contraseña original únicamente en memoria durante la operación de registro.
     * Salidas:
     * Devuelve una cadena con algoritmo, número de iteraciones, sal y hash en Base64.
     * Restricciones:
     * No devuelve la contraseña original ni utiliza sal fija o secretos codificados en la aplicación.
     */

    public string GenerarHash(string contrasena)
    {
        ValidarContrasena(contrasena);

        var salt = RandomNumberGenerator.GetBytes(SaltSizeBytes);
        var hash = DerivarClave(contrasena, salt, Iterations);

        return string.Join(
            Separator,
            Algorithm,
            Iterations.ToString(System.Globalization.CultureInfo.InvariantCulture),
            Convert.ToBase64String(salt),
            Convert.ToBase64String(hash));
    }


    /*
     * Descripción:
     * Verifica si una contraseña en texto claro corresponde a un hash previamente persistido.
     * Entradas:
     * Recibe la contraseña candidata y el hash persistido con metadatos de derivación.
     * Salidas:
     * Devuelve verdadero cuando la clave derivada coincide en comparación de tiempo constante.
     * Restricciones:
     * No modifica datos persistidos y no expone el hash ni la contraseña en respuestas.
     */

    public bool Verificar(string contrasena, string contrasenaHash)
    {
        ValidarContrasena(contrasena);
        if (string.IsNullOrWhiteSpace(contrasenaHash))
        {
            return false;
        }

        var partes = contrasenaHash.Split(Separator);
        if (partes.Length != 4 || partes[0] != Algorithm)
        {
            return false;
        }

        if (!int.TryParse(
                partes[1],
                System.Globalization.NumberStyles.None,
                System.Globalization.CultureInfo.InvariantCulture,
                out var iteraciones)
            || iteraciones <= 0)
        {
            return false;
        }

        var salt = DecodificarBase64(partes[2]);
        var hashEsperado = DecodificarBase64(partes[3]);
        if (salt.Length == 0 || hashEsperado.Length == 0)
        {
            return false;
        }

        var hashCalculado = DerivarClave(contrasena, salt, iteraciones, hashEsperado.Length);
        return CryptographicOperations.FixedTimeEquals(hashCalculado, hashEsperado);
    }

    /*

    * Descripción:
    * Decodifica una cadena en formato Base64 y la convierte en un arreglo de bytes.
    * Entradas:
    * Recibe una cadena de texto en formato Base64.
    * Salidas:
    * Devuelve un arreglo de bytes con el contenido decodificado. Si el valor no es válido, devuelve un arreglo vacío.
    * Restricciones:
    * El valor recibido debe estar en formato Base64 válido para poder ser decodificado correctamente.
      */
    private static byte[] DecodificarBase64(string valor)
    {
        var buffer = new byte[valor.Length];

        return Convert.TryFromBase64String(valor, buffer, out var bytesEscritos)
        ? buffer[..bytesEscritos]
        : Array.Empty<byte>();
    }


    /*
    * Descripción:
    * Genera una clave derivada a partir de una contraseña utilizando PBKDF2 con SHA-256.
    * Entradas:
    * Recibe la contraseña, el salt, la cantidad de iteraciones y el tamaño del hash en bytes.
    * Salidas:
    * Devuelve un arreglo de bytes con la clave derivada.
    * Restricciones:
    * La contraseña debe ser válida, el salt debe existir y la cantidad de iteraciones debe ser suficiente para proteger la clave.
      */

    private static byte[] DerivarClave(
    string contrasena,
    byte[] salt,
    int iteraciones,
    int hashSizeBytes = HashSizeBytes)
    {
        return Rfc2898DeriveBytes.Pbkdf2(
        contrasena,
        salt,
        iteraciones,
        HashAlgorithmName.SHA256,
        hashSizeBytes);
    }


    /*
    * Descripción:
    * Valida que la contraseña tenga contenido antes de procesarla.
    * Entradas:
    * Recibe una contraseña en formato de texto.
    * Salidas:
    * No devuelve valor. Si la contraseña es válida, permite continuar la ejecución.
    * Restricciones:
    * La contraseña no puede ser nula, vacía o contener únicamente espacios en blanco.
      */

    private static void ValidarContrasena(string contrasena)
    {
        if (string.IsNullOrWhiteSpace(contrasena))
        {
            throw new ArgumentException("La contraseña no puede estar vacía.", nameof(contrasena));
        }
    }

}
