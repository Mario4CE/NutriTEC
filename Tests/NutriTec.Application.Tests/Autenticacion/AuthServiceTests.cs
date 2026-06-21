using System.Text;
using NutriTec.Application.Abstractions.Persistence;
using NutriTec.Application.Abstractions.Services;
using NutriTec.Application.Autenticacion;
using NutriTec.Application.Common;
using NutriTec.Contracts.Autenticacion;
using Xunit;

namespace NutriTec.Application.Tests.Autenticacion;

public sealed class AuthServiceTests
{
    [Fact]
    public async Task LoginAsync_CuandoCredencialesSonValidas_RetornaLoginResponseConToken()
    {
        var repository = new FakeAuthRepository();
        var passwordHasher = new FakePasswordHasher();
        var tokenService = new FakeTokenService();
        repository.Credenciales["cliente@example.com"] = new CredencialAutenticacion(
            "1",
            "Cliente Demo",
            "cliente@example.com",
            passwordHasher.GenerarHash("Password123!"),
            "Cliente");
        var service = new AuthService(repository, passwordHasher, tokenService);

        var response = await service.LoginAsync(new LoginRequest { Correo = " cliente@example.com ", Contrasena = "Password123!" }, CancellationToken.None);

        Assert.NotNull(response);
        Assert.Equal("1", response.IdUsuario);
        Assert.Equal("Cliente Demo", response.Nombre);
        Assert.Equal("cliente@example.com", response.Correo);
        Assert.Equal("Cliente", response.TipoUsuario);
        Assert.Equal("jwt-token", response.Token);
        Assert.Equal(tokenService.ExpiraEn, response.ExpiraEn);
    }

    [Fact]
    public async Task LoginAsync_CuandoUsuarioNoExiste_RetornaNull()
    {
        var service = CrearServicio();

        var response = await service.LoginAsync(new LoginRequest { Correo = "nadie@example.com", Contrasena = "Password123!" }, CancellationToken.None);

        Assert.Null(response);
    }

    [Fact]
    public async Task LoginAsync_CuandoContrasenaEsInvalida_RetornaNull()
    {
        var repository = new FakeAuthRepository();
        var passwordHasher = new FakePasswordHasher();
        repository.Credenciales["cliente@example.com"] = new CredencialAutenticacion(
            "1",
            "Cliente Demo",
            "cliente@example.com",
            passwordHasher.GenerarHash("Password123!"),
            "Cliente");
        var service = new AuthService(repository, passwordHasher, new FakeTokenService());

        var response = await service.LoginAsync(new LoginRequest { Correo = "cliente@example.com", Contrasena = "OtraPassword123!" }, CancellationToken.None);

        Assert.Null(response);
    }

    [Fact]
    public async Task RegistrarClienteAsync_CuandoCorreoEstaDisponible_RegistraClienteYRetornaLoginResponse()
    {
        var repository = new FakeAuthRepository();
        var service = new AuthService(repository, new FakePasswordHasher(), new FakeTokenService());
        var request = CrearRegistrarClienteRequest(correo: " cliente@example.com ");

        var response = await service.RegistrarClienteAsync(request, CancellationToken.None);

        Assert.Equal("Cliente", repository.UltimoUsuarioRegistrado?.TipoUsuario);
        Assert.Equal("cliente@example.com", repository.UltimoUsuarioRegistrado?.Correo);
        Assert.Equal("Cliente", response.TipoUsuario);
        Assert.Equal("cliente@example.com", response.Correo);
        Assert.NotEqual(request.Contrasena, repository.UltimoUsuarioRegistrado?.ContrasenaHash);
    }

    [Fact]
    public async Task RegistrarNutricionistaAsync_CuandoCorreoEstaDisponible_RegistraNutricionistaYRetornaLoginResponse()
    {
        var repository = new FakeAuthRepository();
        var service = new AuthService(repository, new FakePasswordHasher(), new FakeTokenService());
        var request = CrearRegistrarNutricionistaRequest(correo: " nutricionista@example.com ");

        var response = await service.RegistrarNutricionistaAsync(request, CancellationToken.None);

        Assert.Equal("Nutricionista", repository.UltimoUsuarioRegistrado?.TipoUsuario);
        Assert.Equal("nutricionista@example.com", repository.UltimoUsuarioRegistrado?.Correo);
        Assert.Equal("Nutricionista", response.TipoUsuario);
        Assert.Equal("nutricionista@example.com", response.Correo);
        Assert.Equal("****-****-****-1111", repository.UltimoUsuarioRegistrado?.TarjetaCredito);
        Assert.DoesNotContain("411111", repository.UltimoUsuarioRegistrado?.TarjetaCredito);
        Assert.NotEqual(request.Contrasena, repository.UltimoUsuarioRegistrado?.ContrasenaHash);
    }

    [Fact]
    public async Task RegistrarClienteAsync_CuandoCorreoYaExiste_LanzaConflictoException()
    {
        var repository = new FakeAuthRepository();
        repository.CorreosExistentes.Add("cliente@example.com");
        var service = new AuthService(repository, new FakePasswordHasher(), new FakeTokenService());

        var exception = await Assert.ThrowsAsync<ConflictoException>(() =>
            service.RegistrarClienteAsync(CrearRegistrarClienteRequest(), CancellationToken.None));

        Assert.Equal("El correo ya está registrado.", exception.Message);
        Assert.Null(repository.UltimoUsuarioRegistrado);
    }

    private static AuthService CrearServicio() => new(
        new FakeAuthRepository(),
        new FakePasswordHasher(),
        new FakeTokenService());

    private static RegistrarClienteRequest CrearRegistrarClienteRequest(string correo = "cliente@example.com") => new(
        Nombre: "Cliente",
        Apellidos: "Demo",
        Edad: 30,
        FechaNacimiento: new DateOnly(1994, 1, 20),
        Peso: 72.5m,
        Imc: 24.1m,
        Pais: "Costa Rica",
        Cintura: 82.0m,
        Cuello: 38.0m,
        Caderas: 95.0m,
        PctMusculo: 42.0m,
        PctGrasa: 18.5m,
        CaloriasDiariasMax: 2200,
        Correo: correo,
        Contrasena: "Password123!");

    private static RegistrarNutricionistaRequest CrearRegistrarNutricionistaRequest(string correo = "nutricionista@example.com") => new()
    {
        Cedula = "1-1111-1111",
        Nombre = "Nutricionista",
        Apellidos = "Demo",
        CodigoNutricionista = "NUT-001",
        Edad = 35,
        FechaNacimiento = new DateOnly(1989, 5, 10),
        Peso = 68.0m,
        Imc = 23.0m,
        Direccion = "San José, Costa Rica",
        FotoUrl = null,
        TarjetaCredito = "411111******1111",
        TipoCobro = "mensual",
        Correo = correo,
        Contrasena = "Password123!"
    };

    private sealed class FakeAuthRepository : IAuthRepository
    {
        public Dictionary<string, CredencialAutenticacion> Credenciales { get; } = new(StringComparer.OrdinalIgnoreCase);

        public HashSet<string> CorreosExistentes { get; } = new(StringComparer.OrdinalIgnoreCase);

        public NuevoUsuarioAutenticacion? UltimoUsuarioRegistrado { get; private set; }

        public Task<CredencialAutenticacion?> ObtenerCredencialPorCorreoAsync(string correo, CancellationToken cancellationToken)
        {
            Credenciales.TryGetValue(correo, out var credencial);
            return Task.FromResult(credencial);
        }

        public Task<bool> ExisteCorreoAsync(string correo, CancellationToken cancellationToken) =>
            Task.FromResult(CorreosExistentes.Contains(correo) || Credenciales.ContainsKey(correo));

        public Task<CredencialAutenticacion> RegistrarUsuarioAsync(NuevoUsuarioAutenticacion usuario, CancellationToken cancellationToken)
        {
            UltimoUsuarioRegistrado = usuario;
            var idUsuario = usuario.TipoUsuario == "Nutricionista" ? usuario.Cedula ?? "1-1111-1111" : "1";
            var credencial = new CredencialAutenticacion(
                idUsuario,
                usuario.Nombre,
                usuario.Correo,
                usuario.ContrasenaHash,
                usuario.TipoUsuario);
            Credenciales[usuario.Correo] = credencial;

            return Task.FromResult(credencial);
        }
    }

    private sealed class FakePasswordHasher : IPasswordHasher
    {
        public string GenerarHash(string contrasena) => $"fake-hash:{Convert.ToBase64String(Encoding.UTF8.GetBytes(contrasena))}";

        public bool Verificar(string contrasena, string hashAlmacenado) => hashAlmacenado == GenerarHash(contrasena);
    }

    private sealed class FakeTokenService : ITokenService
    {
        public DateTimeOffset ExpiraEn { get; } = new(2026, 6, 15, 18, 0, 0, TimeSpan.Zero);

        public TokenAutenticacion GenerarToken(UsuarioTokenAutenticacion usuario) => new("jwt-token", ExpiraEn);
    }
}