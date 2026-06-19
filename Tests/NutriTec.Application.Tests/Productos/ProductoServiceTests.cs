using NutriTec.Application.Abstractions.Persistence;
using NutriTec.Application.Common;
using NutriTec.Application.Productos;
using NutriTec.Contracts.Productos;
using NutriTec.Domain.Productos;
using Xunit;

namespace NutriTec.Application.Tests.Productos;

public sealed class ProductoServiceTests
{
    [Fact]
    public async Task CrearAsync_CuandoDatosSonValidos_CreaProductoPendienteConFichaNutricionalCompleta()
    {
        var repository = new FakeProductoRepository();
        var service = new ProductoService(repository);
        var request = CrearRequest(codigoBarras: " 7441000000011 ");

        var response = await service.CrearAsync(request, CancellationToken.None);

        Assert.Equal("Leche descremada", response.Nombre);
        Assert.Equal("7441000000011", response.CodigoBarras);
        Assert.Equal(250m, response.PorcionGramosMililitros);
        Assert.Equal(120m, response.Calorias);
        Assert.Equal(125m, response.SodioMiligramos);
        Assert.Equal("A, D", response.Vitaminas);
        Assert.Equal(300m, response.CalcioMiligramos);
        Assert.Equal(0.2m, response.HierroMiligramos);
        Assert.False(response.EstaAprobado);
        Assert.NotEqual(Guid.Empty, repository.UltimoProductoCreado?.Id);
    }

    [Fact]
    public async Task CrearAsync_CuandoCodigoBarrasExiste_LanzaConflictoException()
    {
        var repository = new FakeProductoRepository();
        repository.CodigosBarrasExistentes.Add("7441000000011");
        var service = new ProductoService(repository);

        var exception = await Assert.ThrowsAsync<ConflictoException>(() =>
            service.CrearAsync(CrearRequest(), CancellationToken.None));

        Assert.Equal("Ya existe un producto con el código de barras indicado.", exception.Message);
    }

    [Fact]
    public async Task CrearAsync_CuandoPorcionNoEsPositiva_LanzaArgumentException()
    {
        var service = new ProductoService(new FakeProductoRepository());
        var request = CrearRequest(porcion: 0);

        var exception = await Assert.ThrowsAsync<ArgumentException>(() =>
            service.CrearAsync(request, CancellationToken.None));

        Assert.Equal("La porción debe ser mayor que cero.", exception.Message);
    }

    [Fact]
    public async Task CrearAsync_CuandoNutrienteEsNegativo_LanzaArgumentException()
    {
        var service = new ProductoService(new FakeProductoRepository());
        var request = CrearRequest(sodio: -1);

        var exception = await Assert.ThrowsAsync<ArgumentException>(() =>
            service.CrearAsync(request, CancellationToken.None));

        Assert.Equal("Los valores nutricionales no pueden ser negativos.", exception.Message);
    }

    private static CrearProductoRequest CrearRequest(
        string codigoBarras = "7441000000011",
        decimal porcion = 250m,
        decimal sodio = 125m) => new(
            Nombre: " Leche descremada ",
            CodigoBarras: codigoBarras,
            PorcionGramosMililitros: porcion,
            Calorias: 120m,
            Proteinas: 8m,
            Carbohidratos: 12m,
            Grasas: 2m,
            SodioMiligramos: sodio,
            Vitaminas: " A, D ",
            CalcioMiligramos: 300m,
            HierroMiligramos: 0.2m);

    private sealed class FakeProductoRepository : IProductoRepository
    {
        public HashSet<string> CodigosBarrasExistentes { get; } = new(StringComparer.OrdinalIgnoreCase);
        public Producto? UltimoProductoCreado { get; private set; }

        public Task<Producto> CrearAsync(Producto producto, CancellationToken cancellationToken)
        {
            UltimoProductoCreado = producto;
            return Task.FromResult(producto);
        }

        public Task<Producto?> ObtenerPorIdAsync(Guid idProducto, CancellationToken cancellationToken) => Task.FromResult<Producto?>(null);

        public Task<IReadOnlyCollection<Producto>> ListarAsync(CancellationToken cancellationToken) =>
            Task.FromResult<IReadOnlyCollection<Producto>>(Array.Empty<Producto>());

        public Task<IReadOnlyCollection<Producto>> BuscarPorNombreAsync(string nombre, CancellationToken cancellationToken) =>
            Task.FromResult<IReadOnlyCollection<Producto>>(Array.Empty<Producto>());

        public Task<Producto?> ObtenerPorCodigoBarrasAsync(string codigoBarras, CancellationToken cancellationToken) => Task.FromResult<Producto?>(null);

        public Task<bool> ExisteCodigoBarrasAsync(string codigoBarras, Guid? idProductoExcluido, CancellationToken cancellationToken) =>
            Task.FromResult(CodigosBarrasExistentes.Contains(codigoBarras));

        public Task<bool> ActualizarAsync(Producto producto, CancellationToken cancellationToken) => Task.FromResult(true);

        public Task<IReadOnlyCollection<Producto>> ListarPendientesAsync(CancellationToken cancellationToken) =>
            Task.FromResult<IReadOnlyCollection<Producto>>(Array.Empty<Producto>());

        public Task<bool> AprobarAsync(Guid idProducto, CancellationToken cancellationToken) => Task.FromResult(true);

        public Task<bool> EliminarAsync(Guid idProducto, CancellationToken cancellationToken) => Task.FromResult(true);
    }
}
