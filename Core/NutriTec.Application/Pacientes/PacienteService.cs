using NutriTec.Application.Abstractions.Persistence;
using NutriTec.Application.Abstractions.Services;
using NutriTec.Application.Common;
using NutriTec.Contracts.Pacientes;
using NutriTec.Domain.Pacientes;

namespace NutriTec.Application.Pacientes;

public sealed class PacienteService(
    IPacienteRepository repository,
    IUsuarioConsultaRepository usuarioConsultaRepository) : IPacienteService
{
    public async Task<IReadOnlyCollection<ClienteBusquedaResponse>> BuscarClientesAsync(string criterio, CancellationToken cancellationToken)
    {
        ValidarTexto(criterio, nameof(criterio));
        var clientes = await usuarioConsultaRepository.BuscarClientesAsync(criterio.Trim(), cancellationToken);
        return clientes.Select(Mapear).ToArray();
    }

    public async Task<PacienteNutricionistaResponse> AsociarAsync(
        string idNutricionista,
        AsociarPacienteRequest request,
        CancellationToken cancellationToken)
    {
        ValidarTexto(idNutricionista, nameof(idNutricionista));
        ValidarIdentificador(request.IdCliente, nameof(request.IdCliente));

        if (!await usuarioConsultaRepository.EsClienteAsync(request.IdCliente, cancellationToken))
        {
            throw new ArgumentException("El usuario indicado no existe o no es un cliente.", nameof(request.IdCliente));
        }

        if (await repository.ExisteAsociacionAsync(idNutricionista, request.IdCliente, cancellationToken))
        {
            throw new ConflictoException("El cliente ya está asociado como paciente de este nutricionista.");
        }

        var asociacion = new PacienteNutricionista
        {
            Id = Guid.NewGuid(),
            IdNutricionista = idNutricionista,
            IdPaciente = request.IdCliente,
            FechaAsociacionUtc = DateTime.UtcNow
        };

        var creada = await repository.AsociarAsync(asociacion, cancellationToken);
        return Mapear(creada);
    }

    public async Task<IReadOnlyCollection<PacienteNutricionistaResponse>> ListarPorNutricionistaAsync(
        string idNutricionista,
        CancellationToken cancellationToken)
    {
        ValidarTexto(idNutricionista, nameof(idNutricionista));
        var asociaciones = await repository.ListarPorNutricionistaAsync(idNutricionista, cancellationToken);
        return asociaciones.Select(Mapear).ToArray();
    }

    public Task<bool> DesasociarAsync(Guid id, CancellationToken cancellationToken)
    {
        ValidarIdentificador(id, nameof(id));
        return repository.DesasociarAsync(id, cancellationToken);
    }

    private static PacienteNutricionistaResponse Mapear(PacienteNutricionista asociacion)
    {
        return new PacienteNutricionistaResponse(
            asociacion.Id,
            asociacion.IdNutricionista,
            asociacion.IdPaciente,
            asociacion.FechaAsociacionUtc);
    }

    private static ClienteBusquedaResponse Mapear(ClienteResumen cliente)
    {
        return new ClienteBusquedaResponse(
            cliente.Id,
            cliente.Nombre,
            cliente.Apellidos,
            cliente.Correo);
    }

    private static void ValidarIdentificador(Guid identificador, string nombreParametro)
    {
        if (identificador == Guid.Empty)
        {
            throw new ArgumentException("El identificador no puede estar vacío.", nombreParametro);
        }
    }

    private static void ValidarTexto(string texto, string nombreParametro)
    {
        if (string.IsNullOrWhiteSpace(texto))
        {
            throw new ArgumentException("El texto no puede estar vacío.", nombreParametro);
        }
    }
}