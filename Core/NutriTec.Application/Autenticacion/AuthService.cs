using NutriTec.Application.Abstractions.Persistence;
using NutriTec.Application.Abstractions.Services;
using NutriTec.Application.Common;
using NutriTec.Contracts.Autenticacion;

namespace NutriTec.Application.Autenticacion;

/*
 * Descripción:
 * Implementa los casos de uso iniciales de autenticación y registro de usuarios para NutriTEC.
 *
 *
 * Entradas:
 * Recibe DTOs de login y registro, un repositorio de autenticación desacoplado y un servicio de hashing de contraseñas.
 *
 * Salidas:
 * Devuelve respuestas seguras de autenticación sin exponer contraseñas, hashes ni tokens.
 *
 * Restricciones:
 * No emite JWT, no accede directamente a base de datos y no persiste contraseñas en texto plano.
 */

public sealed class AuthService(IAuthRepository repository, IPasswordHasher passwordHasher) : IAuthService
{
    private const string TipoUsuarioCliente = "Cliente";
    private const string TipoUsuarioNutricionista = "Nutricionista";


    /*
     * Descripción:
     * Valida las credenciales de acceso de un usuario registrado.
     * Entradas:
     * Recibe correo, contraseña y token de cancelación desde la API.
     * Salidas:
     * Devuelve la información segura del usuario autenticado o nulo cuando las credenciales no coinciden.
     * Restricciones:
     * No devuelve contraseña, hash ni token JWT.
     */

    public async Task<LoginResponse?> LoginAsync(LoginRequest request, CancellationToken cancellationToken)
    {
        ValidarTexto(request.Correo, nameof(request.Correo));
        ValidarTexto(request.Contrasena, nameof(request.Contrasena));

        var correo = NormalizarTexto(request.Correo);
        var credencial = await repository.ObtenerCredencialPorCorreoAsync(correo, cancellationToken);
        if (credencial is null)
        {
            return null;
        }

        return passwordHasher.Verificar(request.Contrasena, credencial.ContrasenaHash)
            ? MapearLoginResponse(credencial)
            : null;
    }



    /*
     * Descripción:
     * Registra un cliente nuevo con contraseña hasheada.
     * Entradas:
     * Recibe nombre, correo, contraseña inicial y token de cancelación.
     * Salidas:
     * Devuelve la información segura del cliente registrado.
     * Restricciones:
     * El correo debe ser único y la contraseña no se envía al repositorio en texto plano.
     */

    public Task<LoginResponse> RegistrarClienteAsync(
        RegistrarClienteRequest request,
        CancellationToken cancellationToken)
    {
        return RegistrarUsuarioAsync(
            request.Nombre,
            request.Correo,
            request.Contrasena,
            TipoUsuarioCliente,
            cancellationToken);
    }


    /*
     * Descripción:
     * Registra un nutricionista nuevo con contraseña hasheada.
     * Entradas:
     * Recibe nombre, correo, contraseña inicial y token de cancelación.
     * Salidas:
     * Devuelve la información segura del nutricionista registrado.
     * Restricciones:
     * El correo debe ser único y la contraseña no se envía al repositorio en texto plano.
     */

    public Task<LoginResponse> RegistrarNutricionistaAsync(
        RegistrarNutricionistaRequest request,
        CancellationToken cancellationToken)
    {
        return RegistrarUsuarioAsync(
            request.Nombre,
            request.Correo,
            request.Contrasena,
            TipoUsuarioNutricionista,
            cancellationToken);
    }


    /*
     * Descripción:
     * Registra un usuario nuevo en el sistema, normalizando los datos, validando que el correo no exista
     * y almacenando la contraseña de forma hasheada.
     * Entradas:
     * Recibe nombre, correo, contraseña, tipo de usuario y token de cancelación.
     * Salidas:
     * Devuelve la información segura del usuario registrado para iniciar sesión.
     * Restricciones:
     * El nombre, correo y contraseña no pueden estar vacíos. El correo debe ser único.
     * La contraseña se guarda hasheada y no en texto plano.
     */

    private async Task<LoginResponse> RegistrarUsuarioAsync(
        string nombre,
        string correo,
        string contrasena,
        string tipoUsuario,
        CancellationToken cancellationToken)
    {
        ValidarTexto(nombre, nameof(nombre));
        ValidarTexto(correo, nameof(correo));
        ValidarTexto(contrasena, nameof(contrasena));

        var nombreNormalizado = NormalizarTexto(nombre);
        var correoNormalizado = NormalizarTexto(correo);

        if (await repository.ExisteCorreoAsync(correoNormalizado, cancellationToken))
        {
            throw new ConflictoException("Ya existe un usuario registrado con el correo indicado.");
        }

        var usuario = new NuevoUsuarioAutenticacion(
            nombreNormalizado,
            correoNormalizado,
            passwordHasher.GenerarHash(contrasena),
            tipoUsuario);

        var credencial = await repository.RegistrarUsuarioAsync(usuario, cancellationToken);

        return MapearLoginResponse(credencial);
    }


    /*
     * Descripción:
     * Convierte una credencial de autenticación en una respuesta segura de inicio de sesión.
     * Entradas:
     * Recibe una credencial de autenticación con los datos del usuario registrado.
     * Salidas:
     * Devuelve un objeto LoginResponse con el identificador, nombre, correo y tipo de usuario.
     * Restricciones:
     * No debe incluir información sensible como la contraseña o el hash de la contraseña.
     */

    private static LoginResponse MapearLoginResponse(CredencialAutenticacion credencial)
    {
        return new LoginResponse(
            credencial.IdUsuario,
            credencial.Nombre,
            credencial.Correo,
            credencial.TipoUsuario);
    }

    /*
     * Descripción:
     * Elimina los espacios en blanco al inicio y al final de un texto.
     * Entradas:
     * Recibe una cadena de texto.
     * Salidas:
     * Devuelve el texto normalizado sin espacios externos.
     * Restricciones:
     * El texto recibido no debe ser nulo.
     */

    private static string NormalizarTexto(string texto)
    {
        return texto.Trim();
    }

    /*
     * Descripción:
     * Valida que una cadena de texto no esté vacía, nula o compuesta solo por espacios.
     * Entradas:
     * Recibe el texto a validar y el nombre del parámetro asociado.
     * Salidas:
     * No devuelve valor. Si el texto es válido, permite continuar la ejecución.
     * Restricciones:
     * Si el texto está vacío, nulo o solo contiene espacios, lanza una excepción ArgumentException.
     */

    private static void ValidarTexto(string texto, string nombreParametro)
    {
        if (string.IsNullOrWhiteSpace(texto))
        {
            throw new ArgumentException("El texto no puede estar vacío.", nombreParametro);
        }
    }
}
