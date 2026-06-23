using System.Text;
using NutriTec.Application.Abstractions.Persistence;
using NutriTec.Application.Abstractions.Services;
using NutriTec.Application.Common;
using NutriTec.Contracts.Autenticacion;

namespace NutriTec.Application.Autenticacion;

public sealed class AuthService(
    IAuthRepository authRepository,
    IPasswordHasher passwordHasher,
    ITokenService tokenService) : IAuthService
{
    public async Task<LoginResponse?> LoginAsync(LoginRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Correo) || string.IsNullOrWhiteSpace(request.Contrasena))
        {
            return null;
        }

        var correo = request.Correo.Trim();
        var credencial = await authRepository.ObtenerCredencialPorCorreoAsync(correo, cancellationToken);

        if (credencial is null || !passwordHasher.Verificar(request.Contrasena, credencial.ContrasenaHash))
        {
            return null;
        }

        return MapearLoginResponse(credencial);
    }

    public async Task<LoginResponse> RegistrarClienteAsync(RegistrarClienteRequest request, CancellationToken cancellationToken)
    {
        var correo = request.Correo.Trim();
        await ValidarCorreoDisponibleAsync(correo, cancellationToken);

        var nuevoUsuario = new NuevoUsuarioAutenticacion(
            TipoUsuario: "Cliente",
            Nombre: request.Nombre.Trim(),
            Apellidos: request.Apellidos.Trim(),
            Edad: request.Edad,
            FechaNacimiento: request.FechaNacimiento,
            Peso: request.Peso,
            Imc: request.Imc,
            Correo: correo,
            ContrasenaHash: passwordHasher.GenerarHash(request.Contrasena),
            Pais: request.Pais.Trim(),
            Cintura: request.Cintura,
            Cuello: request.Cuello,
            Caderas: request.Caderas,
            PctMusculo: request.PctMusculo,
            PctGrasa: request.PctGrasa,
            CaloriasDiariasMax: request.CaloriasDiariasMax);

        var credencial = await authRepository.RegistrarUsuarioAsync(nuevoUsuario, cancellationToken);

        return MapearLoginResponse(credencial);
    }

    public async Task<LoginResponse> RegistrarNutricionistaAsync(RegistrarNutricionistaRequest request, CancellationToken cancellationToken)
    {
        var correo = request.Correo.Trim();
        await ValidarCorreoDisponibleAsync(correo, cancellationToken);

        var nuevoUsuario = new NuevoUsuarioAutenticacion(
            TipoUsuario: "Nutricionista",
            Nombre: request.Nombre.Trim(),
            Apellidos: request.Apellidos.Trim(),
            Edad: request.Edad,
            FechaNacimiento: request.FechaNacimiento,
            Peso: request.Peso,
            Imc: request.Imc,
            Correo: correo,
            ContrasenaHash: passwordHasher.GenerarHash(request.Contrasena),
            Cedula: request.Cedula.Trim(),
            CodigoNutricionista: request.CodigoNutricionista.Trim(),
            Direccion: request.Direccion.Trim(),
            FotoUrl: string.IsNullOrWhiteSpace(request.FotoUrl) ? null : request.FotoUrl.Trim(),
            TarjetaCredito: EnmascararTarjetaCredito(request.TarjetaCredito),
            TipoCobro: request.TipoCobro.Trim());

        var credencial = await authRepository.RegistrarUsuarioAsync(nuevoUsuario, cancellationToken);

        return MapearLoginResponse(credencial);
    }

    private static string EnmascararTarjetaCredito(string tarjetaCredito)
    {
        if (string.IsNullOrWhiteSpace(tarjetaCredito))
        {
            throw new ArgumentException("La tarjeta de crédito es obligatoria.", nameof(tarjetaCredito));
        }

        var digitos = new StringBuilder(capacity: tarjetaCredito.Length);
        foreach (var caracter in tarjetaCredito)
        {
            if (char.IsDigit(caracter))
            {
                digitos.Append(caracter);
            }
        }

        if (digitos.Length < 4)
        {
            throw new ArgumentException("La tarjeta de crédito debe incluir al menos los últimos 4 dígitos.", nameof(tarjetaCredito));
        }

        var ultimosCuatroDigitos = digitos.ToString(digitos.Length - 4, 4);
        return $"****-****-****-{ultimosCuatroDigitos}";
    }

    private async Task ValidarCorreoDisponibleAsync(string correo, CancellationToken cancellationToken)
    {
        if (await authRepository.ExisteCorreoAsync(correo, cancellationToken))
        {
            throw new ConflictoException("El correo ya está registrado.");
        }
    }

    private LoginResponse MapearLoginResponse(CredencialAutenticacion credencial)
    {
        var token = tokenService.GenerarToken(new UsuarioTokenAutenticacion(
            credencial.IdUsuario,
            credencial.Nombre,
            credencial.Correo,
            credencial.TipoUsuario));

        return new LoginResponse(
            credencial.IdUsuario,
            credencial.Nombre,
            credencial.Correo,
            credencial.TipoUsuario,
            token.Token,
            token.ExpiraEn,
            credencial.Peso,
            credencial.Imc,
            credencial.CaloriasDiariasMax,
            credencial.Apellidos,
            credencial.Edad,
            credencial.Pais);
    }
}
