using NutriTec.Application.Abstractions.Persistence;
using NutriTec.Application.Abstractions.Services;
using NutriTec.Application.Administracion;
using NutriTec.Contracts.ObjetosSql;
using NutriTec.Domain.Productos;
using Xunit;

namespace NutriTec.Application.Tests.Administracion;

public sealed class AdministracionServiceTests
{
    [Fact]
    public async Task ListarProductosPendientesAsync_MapeaProductosPendientes()
    {
        var producto = new Producto
        {
            Id = Guid.NewGuid(),
            Nombre = "Producto pendiente",
            CodigoBarras = "123",
            PorcionGramosMililitros = 100m,
            Calorias = 50m,
            Proteinas = 2m,
            Carbohidratos = 10m,
            Grasas = 1m,
            SodioMiligramos = 20m,
            EstaAprobado = false,
            FechaCreacionUtc = DateTime.UtcNow
        };
        var repository = new FakeAdministracionRepository { ProductosPendientes = new[] { producto } };
        var service = new AdministracionService(repository, new FakeObjetosSqlService());

        var pendientes = await service.ListarProductosPendientesAsync(CancellationToken.None);

        var pendiente = Assert.Single(pendientes);
        Assert.Equal(producto.Id, pendiente.Id);
        Assert.Equal(producto.Nombre, pendiente.Nombre);
        Assert.False(pendiente.EstaAprobado);
    }

    [Fact]
    public async Task AprobarProductoAsync_CuandoIdentificadorEsValido_DelegaAlRepositorio()
    {
        var idProducto = Guid.NewGuid();
        var repository = new FakeAdministracionRepository { ResultadoAprobacion = true };
        var service = new AdministracionService(repository, new FakeObjetosSqlService());

        var aprobado = await service.AprobarProductoAsync(idProducto, CancellationToken.None);

        Assert.True(aprobado);
        Assert.Equal(idProducto, repository.UltimoProductoAprobado);
    }

    [Fact]
    public async Task AprobarProductoAsync_CuandoIdentificadorEstaVacio_LanzaArgumentException()
    {
        var service = new AdministracionService(new FakeAdministracionRepository(), new FakeObjetosSqlService());

        await Assert.ThrowsAsync<ArgumentException>(() =>
            service.AprobarProductoAsync(Guid.Empty, CancellationToken.None));
    }

    private sealed class FakeObjetosSqlService : IObjetosSqlService
    {
        public Task<IReadOnlyCollection<ReporteCobroNutricionistaResponse>> ObtenerReporteCobroNutricionistasAsync(
            decimal montoBasePorPaciente,
            bool incluirSinPacientes,
            CancellationToken cancellationToken) =>
            Task.FromResult<IReadOnlyCollection<ReporteCobroNutricionistaResponse>>(Array.Empty<ReporteCobroNutricionistaResponse>());

        public Task<ProductoAprobadoSqlResponse?> AprobarProductoConProcedimientoAsync(Guid idProducto, CancellationToken cancellationToken) =>
            Task.FromResult<ProductoAprobadoSqlResponse?>(null);

        public Task<AsignarPlanPacienteResponse> AsignarPlanPacienteAsync(
            int idPlan,
            int idUsuario,
            AsignarPlanPacienteRequest request,
            CancellationToken cancellationToken) =>
            Task.FromResult(new AsignarPlanPacienteResponse(1));

        public Task<RegistrarMedidaUsuarioResponse> RegistrarMedidaUsuarioAsync(
            int idUsuario,
            RegistrarMedidaUsuarioRequest request,
            CancellationToken cancellationToken) =>
            Task.FromResult(new RegistrarMedidaUsuarioResponse(1, idUsuario, request.Fecha, request.PesoKg, 22.5m));

        public Task<CalcularImcResponse> CalcularImcAsync(decimal pesoKg, decimal estaturaCm, CancellationToken cancellationToken) =>
            Task.FromResult(new CalcularImcResponse(pesoKg, estaturaCm, 22.86m));

        public Task<TotalCaloriasPlanResponse> ObtenerTotalCaloriasPlanAsync(int idPlan, CancellationToken cancellationToken) =>
            Task.FromResult(new TotalCaloriasPlanResponse(idPlan, 0m));
    }

    private sealed class FakeAdministracionRepository : IAdministracionRepository
    {
        public IReadOnlyCollection<Producto> ProductosPendientes { get; init; } = Array.Empty<Producto>();
        public bool ResultadoAprobacion { get; init; }
        public Guid? UltimoProductoAprobado { get; private set; }

        public Task<IReadOnlyCollection<Producto>> ListarProductosPendientesAsync(CancellationToken cancellationToken) =>
            Task.FromResult(ProductosPendientes);

        public Task<bool> AprobarProductoAsync(Guid idProducto, CancellationToken cancellationToken)
        {
            UltimoProductoAprobado = idProducto;
            return Task.FromResult(ResultadoAprobacion);
        }
    }
}
