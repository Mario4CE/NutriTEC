using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using NutriTec.Application.Abstractions.Services;
using NutriTec.Infrastructure.Sql.Bootstrap;
using NutriTec.Infrastructure.Sql.Persistence;
using NutriTec.Infrastructure.Sql.Persistence.Entities;
using Xunit;

namespace NutriTec.Infrastructure.Sql.Tests.Bootstrap;

public sealed class AdminBootstrapServiceTests
{
    [Fact]
    public async Task InicializarAsync_CuandoBootstrapEstaDeshabilitado_NoCreaAdministrador()
    {
        await using var dbContext = CrearDbContext();
        var hasher = new FakePasswordHasher();
        var service = CrearService(dbContext, hasher, new AdminBootstrapOptions
        {
            Enabled = false,
            Email = string.Empty,
            Password = string.Empty
        });

        await service.InicializarAsync();

        Assert.False(await dbContext.Administradores.AnyAsync());
        Assert.Equal(0, hasher.GenerarHashLlamadas);
    }

    [Theory]
    [InlineData("", "Password123!")]
    [InlineData("admin@nutritec.example", "")]
    public async Task InicializarAsync_CuandoConfiguracionHabilitadaEstaIncompleta_LanzaInvalidOperationException(
        string email,
        string password)
    {
        await using var dbContext = CrearDbContext();
        var service = CrearService(dbContext, new FakePasswordHasher(), new AdminBootstrapOptions
        {
            Enabled = true,
            Email = email,
            Password = password
        });

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => service.InicializarAsync());

        Assert.Equal("La configuración de bootstrap del administrador está incompleta.", exception.Message);
    }

    [Fact]
    public async Task InicializarAsync_CuandoNoExisteAdministrador_CreaAdministradorConPasswordHash()
    {
        await using var dbContext = CrearDbContext();
        var hasher = new FakePasswordHasher();
        var service = CrearService(dbContext, hasher, new AdminBootstrapOptions
        {
            Enabled = true,
            Email = "  admin@nutritec.example  ",
            Password = "Password123!"
        });

        await service.InicializarAsync();

        var administradores = await dbContext.Administradores.ToListAsync();
        var administrador = Assert.Single(administradores);
        Assert.Equal("admin@nutritec.example", administrador.Email);
        Assert.Equal("HASH::Password123!", administrador.PasswordHash);
        Assert.Equal(1, hasher.GenerarHashLlamadas);
    }

    [Fact]
    public async Task InicializarAsync_CuandoYaExisteAdministrador_NoCreaOtroAdministrador()
    {
        await using var dbContext = CrearDbContext();
        dbContext.Administradores.Add(new AdministradorSql
        {
            Email = "existente@nutritec.example",
            PasswordHash = "hash-existente"
        });
        await dbContext.SaveChangesAsync();

        var hasher = new FakePasswordHasher();
        var service = CrearService(dbContext, hasher, new AdminBootstrapOptions
        {
            Enabled = true,
            Email = "admin@nutritec.example",
            Password = "Password123!"
        });

        await service.InicializarAsync();

        Assert.Equal(1, await dbContext.Administradores.CountAsync());
        Assert.Equal(0, hasher.GenerarHashLlamadas);
    }

    private static AdminBootstrapService CrearService(
        NutriTecDbContext dbContext,
        IPasswordHasher passwordHasher,
        AdminBootstrapOptions options) => new(
            dbContext,
            passwordHasher,
            Options.Create(options));

    private static NutriTecDbContext CrearDbContext()
    {
        var options = new DbContextOptionsBuilder<NutriTecDbContext>()
            .UseInMemoryDatabase($"AdminBootstrapTests-{Guid.NewGuid()}")
            .Options;

        return new NutriTecDbContext(options);
    }

    private sealed class FakePasswordHasher : IPasswordHasher
    {
        public int GenerarHashLlamadas { get; private set; }

        public string GenerarHash(string contrasena)
        {
            GenerarHashLlamadas++;
            return $"HASH::{contrasena}";
        }

        public bool Verificar(string contrasena, string hashAlmacenado) => hashAlmacenado == $"HASH::{contrasena}";
    }
}
