using NutriTec.Application.Abstractions.Persistence;
using NutriTec.Application.Retroalimentaciones;
using NutriTec.Contracts.Retroalimentaciones;
using NutriTec.Domain.Retroalimentaciones;
using Xunit;

namespace NutriTec.Application.Tests.Retroalimentaciones;

public sealed class RetroalimentacionServiceTests
{
    [Fact]
    public async Task CrearAsync_CuandoDatosSonValidos_CreaForoConMensajeInicial()
    {
        var repository = new FakeRetroalimentacionRepository();
        var service = new RetroalimentacionService(repository);
        var idPaciente = Guid.NewGuid();
        var idNutricionista = Guid.NewGuid();

        var response = await service.CrearAsync(
            new CrearRetroalimentacionRequest(idPaciente, idNutricionista, " Paciente ", " Mensaje inicial "),
            CancellationToken.None);

        Assert.NotEqual(Guid.Empty, response.Id);
        Assert.Equal(idPaciente, response.IdPaciente);
        Assert.Equal(idNutricionista, response.IdNutricionista);
        var mensaje = Assert.Single(response.Mensajes);
        Assert.Equal("Paciente", mensaje.Autor);
        Assert.Equal("Mensaje inicial", mensaje.Mensaje);
    }

    [Fact]
    public async Task CrearAsync_CuandoPacienteEstaVacio_LanzaArgumentException()
    {
        var service = new RetroalimentacionService(new FakeRetroalimentacionRepository());

        await Assert.ThrowsAsync<ArgumentException>(() =>
            service.CrearAsync(
                new CrearRetroalimentacionRequest(Guid.Empty, Guid.NewGuid(), "Paciente", "Mensaje"),
                CancellationToken.None));
    }

    [Fact]
    public async Task ResponderAsync_CuandoDatosSonValidos_AgregaMensajeAlRepositorio()
    {
        var repository = new FakeRetroalimentacionRepository { ResultadoAgregarMensaje = true };
        var service = new RetroalimentacionService(repository);
        var idRetroalimentacion = Guid.NewGuid();

        var actualizada = await service.ResponderAsync(
            idRetroalimentacion,
            new ResponderRetroalimentacionRequest(" Nutricionista ", " Respuesta "),
            CancellationToken.None);

        Assert.True(actualizada);
        Assert.Equal(idRetroalimentacion, repository.UltimaRetroalimentacionRespondida);
        Assert.Equal("Nutricionista", repository.UltimoMensajeAgregado?.Autor);
        Assert.Equal("Respuesta", repository.UltimoMensajeAgregado?.Mensaje);
    }

    private sealed class FakeRetroalimentacionRepository : IRetroalimentacionRepository
    {
        public bool ResultadoAgregarMensaje { get; init; }
        public Guid? UltimaRetroalimentacionRespondida { get; private set; }
        public MensajeRetroalimentacion? UltimoMensajeAgregado { get; private set; }

        public Task<Retroalimentacion> CrearAsync(Retroalimentacion retroalimentacion, CancellationToken cancellationToken) =>
            Task.FromResult(retroalimentacion);

        public Task<IReadOnlyCollection<Retroalimentacion>> ObtenerPorPacienteAsync(Guid idPaciente, CancellationToken cancellationToken) =>
            Task.FromResult<IReadOnlyCollection<Retroalimentacion>>(Array.Empty<Retroalimentacion>());

        public Task<IReadOnlyCollection<Retroalimentacion>> ObtenerPorNutricionistaAsync(Guid idNutricionista, CancellationToken cancellationToken) =>
            Task.FromResult<IReadOnlyCollection<Retroalimentacion>>(Array.Empty<Retroalimentacion>());

        public Task<bool> AgregarMensajeAsync(Guid idRetroalimentacion, MensajeRetroalimentacion mensaje, CancellationToken cancellationToken)
        {
            UltimaRetroalimentacionRespondida = idRetroalimentacion;
            UltimoMensajeAgregado = mensaje;
            return Task.FromResult(ResultadoAgregarMensaje);
        }
    }
}
