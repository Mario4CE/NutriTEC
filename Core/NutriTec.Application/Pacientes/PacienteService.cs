using NutriTec.Application.Abstractions.Persistence;
using NutriTec.Application.Abstractions.Services;
using NutriTec.Application.Common;
using NutriTec.Contracts.Pacientes;
using NutriTec.Domain.Pacientes;

namespace NutriTec.Application.Pacientes;

/*
 * Descripción:
 * Implementa las reglas de aplicación para buscar clientes y asociarlos como pacientes
 * de un nutricionista.
 *
 * Entradas:
 * Recibe DTOs de solicitud y repositorios desacoplados de persistencia.
 *
 * Salidas:
 * Devuelve DTOs preparados para la API o confirma modificaciones.
 *
 * Restricciones:
 * Rechaza identificadores vacíos, criterios sin contenido y asociaciones duplicadas.
 */

public sealed class PacienteService(
    IPacienteRepository repository,
    IUsuarioConsultaRepository usuarioConsultaRepository) : IPacienteService
{
    /*
     * Descripción: Busca clientes candidatos a ser pacientes.
     * Entradas: Criterio de búsqueda y token de cancelación.
     * Salidas: Devuelve clientes coincidentes como DTOs.
     * Restricciones: El criterio no puede estar vacío.
     */

    public async Task<IReadOnlyCollection<ClienteBusquedaResponse>> BuscarClientesAsync(string criterio, CancellationToken cancellationToken)
    {
        ValidarTexto(criterio, nameof(criterio));
        var clientes = await usuarioConsultaRepository.BuscarClientesAsync(criterio.Trim(), cancellationToken);
        return clientes.Select(Mapear).ToArray();
    }

    /*
     * Descripción: Asocia un cliente como paciente de un nutricionista.
     * Entradas: Identificador del nutricionista, DTO de solicitud y token de cancelación.
     * Salidas: Devuelve la asociación creada como DTO.
     * Restricciones: El cliente debe existir y no puede estar ya asociado al mismo nutricionista.
     */

    public async Task<PacienteNutricionistaResponse> AsociarAsync(
        Guid idNutricionista,
        AsociarPacienteRequest request,
        CancellationToken cancellationToken)
    {
        ValidarIdentificador(idNutricionista, nameof(idNutricionista));
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

    /*
     * Descripción: Lista los pacientes asociados a un nutricionista.
     * Entradas: Identificador del nutricionista y token de cancelación.
     * Salidas: Devuelve una colección de DTOs.
     * Restricciones: El identificador no puede ser vacío.
     */

    public async Task<IReadOnlyCollection<PacienteNutricionistaResponse>> ListarPorNutricionistaAsync(
        Guid idNutricionista,
        CancellationToken cancellationToken)
    {
        ValidarIdentificador(idNutricionista, nameof(idNutricionista));
        var asociaciones = await repository.ListarPorNutricionistaAsync(idNutricionista, cancellationToken);
        return asociaciones.Select(Mapear).ToArray();
    }

    /*
     * Descripción: Elimina la asociación entre un nutricionista y un paciente.
     * Entradas: Identificador de la asociación y token de cancelación.
     * Salidas: Devuelve verdadero cuando la asociación fue eliminada.
     * Restricciones: El identificador no puede ser vacío.
     */

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