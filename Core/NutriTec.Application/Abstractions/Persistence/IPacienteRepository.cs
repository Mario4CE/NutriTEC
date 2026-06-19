using NutriTec.Domain.Pacientes;

namespace NutriTec.Application.Abstractions.Persistence;

public interface IPacienteRepository
{
    Task<PacienteNutricionista> AsociarAsync(PacienteNutricionista asociacion, CancellationToken cancellationToken);

    Task<bool> ExisteAsociacionAsync(string idNutricionista, Guid idPaciente, CancellationToken cancellationToken);

    Task<IReadOnlyCollection<PacienteNutricionista>> ListarPorNutricionistaAsync(string idNutricionista, CancellationToken cancellationToken);

    Task<PacienteNutricionista?> ObtenerPorIdAsync(Guid id, CancellationToken cancellationToken);

    Task<bool> EsPacienteDeAsync(string idNutricionista, Guid idPaciente, CancellationToken cancellationToken);

    Task<bool> DesasociarAsync(Guid id, CancellationToken cancellationToken);
}