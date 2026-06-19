using NutriTec.Contracts.Pacientes;

namespace NutriTec.Application.Abstractions.Services;

public interface IPacienteService
{
    Task<IReadOnlyCollection<ClienteBusquedaResponse>> BuscarClientesAsync(string criterio, CancellationToken cancellationToken);

    Task<PacienteNutricionistaResponse> AsociarAsync(string idNutricionista, AsociarPacienteRequest request, CancellationToken cancellationToken);

    Task<IReadOnlyCollection<PacienteNutricionistaResponse>> ListarPorNutricionistaAsync(string idNutricionista, CancellationToken cancellationToken);

    Task<bool> DesasociarAsync(Guid id, CancellationToken cancellationToken);
}