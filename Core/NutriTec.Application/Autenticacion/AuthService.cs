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
     * Registra un cliente nuevo con contraseña hasheada y datos requeridos por el esquema SQL de usuarios.
     * Entradas:
     * Recibe datos personales, métricas corporales, correo, contraseña inicial y token de cancelación.
     * Salidas:
     * Devuelve la información segura del cliente registrado.
     * Restricciones:
     * El correo debe ser único y la contraseña no se envía al repositorio en texto plano.
     */

    public Task<LoginResponse> RegistrarClienteAsync(
        RegistrarClienteRequest request,
        CancellationToken cancellationToken)
    {
        ValidarTexto(request.Nombre, nameof(request.Nombre));
        ValidarTexto(request.Apellidos, nameof(request.Apellidos));
        ValidarTexto(request.Pais, nameof(request.Pais));
        ValidarTexto(request.Correo, nameof(request.Correo));
        ValidarTexto(request.Contrasena, nameof(request.Contrasena));
        ValidarMayorQueCero(request.Peso, nameof(request.Peso));
        ValidarMayorQueCero(request.Imc, nameof(request.Imc));
        ValidarMayorQueCero(request.CaloriasDiariasMax, nameof(request.CaloriasDiariasMax));
        ValidarEnteroNoNegativo(request.Edad, nameof(request.Edad));

        var usuario = new NuevoUsuarioAutenticacion(
            Cedula: null,
            Nombre: NormalizarTexto(request.Nombre),
            Apellidos: NormalizarTexto(request.Apellidos),
            Edad: request.Edad,
            FechaNacimiento: request.FechaNacimiento,
            Peso: request.Peso,
            Imc: request.Imc,
            Pais: NormalizarTexto(request.Pais),
            Cintura: request.Cintura,
            Cuello: request.Cuello,
            Caderas: request.Caderas,
            PctMusculo: request.PctMusculo,
            PctGrasa: request.PctGrasa,
            CaloriasDiariasMax: request.CaloriasDiariasMax,
            CodigoNutricionista: null,
            Direccion: null,
            FotoUrl: null,
            TarjetaCredito: null,
            TipoCobro: null,
            Correo: NormalizarTexto(request.Correo),
            ContrasenaHash: passwordHasher.GenerarHash(request.Contrasena),
            TipoUsuario: TipoUsuarioCliente);

        return RegistrarUsuarioAsync(usuario, cancellationToken);
    }


    /*
     * Descripción:
     * Registra un nutricionista nuevo con contraseña hasheada y datos requeridos por el esquema SQL de nutricionistas.
     * Entradas:
     * Recibe identificación, datos personales, datos profesionales, correo, contraseña inicial y token de cancelación.
     * Salidas:
     * Devuelve la información segura del nutricionista registrado.
     * Restricciones:
     * El correo debe ser único y la contraseña no se envía al repositorio en texto plano.
     */

    public Task<LoginResponse> RegistrarNutricionistaAsync(
        RegistrarNutricionistaRequest request,
        CancellationToken cancellationToken)
    {
        ValidarTexto(request.Cedula, nameof(request.Cedula));
        ValidarTexto(request.Nombre, nameof(request.Nombre));
        ValidarTexto(request.Apellidos, nameof(request.Apellidos));
        ValidarTexto(request.CodigoNutricionista, nameof(request.CodigoNutricionista));
        ValidarTexto(request.Direccion, nameof(request.Direccion));
        ValidarTexto(request.TarjetaCredito, nameof(request.TarjetaCredito));
        ValidarTexto(request.TipoCobro, nameof(request.TipoCobro));
        ValidarTexto(request.Correo, nameof(request.Correo));
        ValidarTexto(request.Contrasena, nameof(request.Contrasena));
        ValidarMayorQueCero(request.Peso, nameof(request.Peso));
        ValidarMayorQueCero(request.Imc, nameof(request.Imc));
        ValidarEnteroNoNegativo(request.Edad, nameof(request.Edad));
        ValidarTipoCobro(request.TipoCobro);

        var usuario = new NuevoUsuarioAutenticacion(
            Cedula: NormalizarTexto(request.Cedula),
            Nombre: NormalizarTexto(request.Nombre),
            Apellidos: NormalizarTexto(request.Apellidos),
            Edad: request.Edad,
            FechaNacimiento: request.FechaNacimiento,
            Peso: request.Peso,
            Imc: request.Imc,
            Pais: null,
            Cintura: null,
            Cuello: null,
            Caderas: null,
            PctMusculo: null,
            PctGrasa: null,
            CaloriasDiariasMax: null,
            CodigoNutricionista: NormalizarTexto(request.CodigoNutricionista),
            Direccion: NormalizarTexto(request.Direccion),
            FotoUrl: NormalizarTextoOpcional(request.FotoUrl),
            TarjetaCredito: NormalizarTexto(request.TarjetaCredito),
            TipoCobro: NormalizarTexto(request.TipoCobro),
            Correo: NormalizarTexto(request.Correo),
            ContrasenaHash: passwordHasher.GenerarHash(request.Contrasena),
            TipoUsuario: TipoUsuarioNutricionista);

        return RegistrarUsuarioAsync(usuario, cancellationToken);
    }


    /*
     * Descripción:
     * Registra un usuario nuevo en el sistema, validando que el correo no exista y almacenando la contraseña de forma hasheada.
     * Entradas:
     * Recibe datos normalizados del usuario con contraseña hasheada y token de cancelación.
     * Salidas:
     * Devuelve la información segura del usuario registrado para iniciar sesión.
     * Restricciones:
     * El correo debe ser único. La contraseña se guarda hasheada y no en texto plano.
     */

    private async Task<LoginResponse> RegistrarUsuarioAsync(
        NuevoUsuarioAutenticacion usuario,
        CancellationToken cancellationToken)
    {
        if (await repository.ExisteCorreoAsync(usuario.Correo, cancellationToken))
        {
            throw new ConflictoException("Ya existe un usuario registrado con el correo indicado.");
        }

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
     * Elimina espacios externos de un texto opcional y conserva nulos cuando no hay valor útil.
     * Entradas:
     * Recibe una cadena opcional.
     * Salidas:
     * Devuelve texto normalizado o nulo.
     * Restricciones:
     * No debe convertir valores vacíos en datos persistibles.
     */

    private static string? NormalizarTextoOpcional(string? texto)
    {
        return string.IsNullOrWhiteSpace(texto) ? null : texto.Trim();
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

    /*
     * Descripción:
     * Valida que un valor decimal sea mayor que cero.
     * Entradas:
     * Recibe el valor decimal y el nombre del parámetro asociado.
     * Salidas:
     * No devuelve valor. Si el valor es válido, permite continuar la ejecución.
     * Restricciones:
     * Si el valor es cero o negativo, lanza una excepción ArgumentOutOfRangeException.
     */

    private static void ValidarMayorQueCero(decimal valor, string nombreParametro)
    {
        if (valor <= 0)
        {
            throw new ArgumentOutOfRangeException(nombreParametro, "El valor debe ser mayor que cero.");
        }
    }

    /*
     * Descripción:
     * Valida que un valor entero no sea negativo.
     * Entradas:
     * Recibe el valor entero y el nombre del parámetro asociado.
     * Salidas:
     * No devuelve valor. Si el valor es válido, permite continuar la ejecución.
     * Restricciones:
     * Si el valor es negativo, lanza una excepción ArgumentOutOfRangeException.
     */

    private static void ValidarEnteroNoNegativo(int valor, string nombreParametro)
    {
        if (valor < 0)
        {
            throw new ArgumentOutOfRangeException(nombreParametro, "El valor no puede ser negativo.");
        }
    }

    /*
     * Descripción:
     * Valida que el tipo de cobro de un nutricionista coincida con los valores permitidos por SQL.
     * Entradas:
     * Recibe el tipo de cobro.
     * Salidas:
     * No devuelve valor. Si el valor es válido, permite continuar la ejecución.
     * Restricciones:
     * Solo se permiten semanal, mensual o anual.
     */

    private static void ValidarTipoCobro(string tipoCobro)
    {
        var tipoNormalizado = NormalizarTexto(tipoCobro);
        if (!string.Equals(tipoNormalizado, "semanal", StringComparison.Ordinal)
            && !string.Equals(tipoNormalizado, "mensual", StringComparison.Ordinal)
            && !string.Equals(tipoNormalizado, "anual", StringComparison.Ordinal))
        {
            throw new ArgumentException("El tipo de cobro debe ser semanal, mensual o anual.", nameof(tipoCobro));
        }
    }
}
