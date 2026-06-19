using NutriTec.Application.Abstractions.Persistence;
using NutriTec.Application.Abstractions.Services;
using NutriTec.Application.Common;
using NutriTec.Contracts.Planes;
using NutriTec.Domain.Planes;

namespace NutriTec.Application.Planes;

public sealed class AsignacionPlanService(
    IAsignacionPlanRepository repository,
    IPlanRepository planRepository,
    IPacienteRepository pacienteRepository) : IAsignacionPlanService
{
    public async Task<AsignacionPlanResponse> AsignarAsync(
        string idNutricionista,
        AsignarPlanRequest request,
        CancellationToken cancellationToken)
    {
        ValidarTexto(idNutricionista, nameof(idNutricionista));
        ValidarIdentificador(request.IdPaciente, nameof(request.IdPaciente));
        ValidarIdentificador(request.IdPlan, nameof(request.IdPlan));

        if (request.FechaInicio > request.FechaFin)
        {
            throw new ArgumentException("La fecha de inicio no puede ser posterior a la fecha de fin.", nameof(request.FechaInicio));
        }

        if (!await pacienteRepository.EsPacienteDeAsync(idNutricionista, request.IdPaciente, cancellationToken))
        {
            throw new ConflictoException("El paciente no está asociado a este nutricionista.");
        }

        if (!await planRepository.PertenecePlanAAsync(request.IdPlan, idNutricionista, cancellationToken))
        {
            throw new ConflictoException("El plan no existe o no pertenece a este nutricionista.");
        }

        var asignacion = new AsignacionPlan
        {
            Id = Guid.NewGuid(),
            IdPaciente = request.IdPaciente,
            IdPlan = request.IdPlan,
            IdNutricionista = idNutricionista,
            FechaInicio = request.FechaInicio,
            FechaFin = request.FechaFin,
            FechaAsignacionUtc = DateTime.UtcNow
        };

        var creada = await repository.AsignarAsync(asignacion, cancellationToken);
        return Mapear(creada);
    }

    public async Task<AsignacionPlanResponse?> ObtenerVigentePorPacienteAsync(Guid idPaciente, CancellationToken cancellationToken)
    {
        ValidarIdentificador(idPaciente, nameof(idPaciente));
        var asignacion = await repository.ObtenerVigentePorPacienteAsync(idPaciente, cancellationToken);
        return asignacion is null ? null : Mapear(asignacion);
    }

    public async Task<IReadOnlyCollection<AsignacionPlanResponse>> ListarPorPacienteAsync(Guid idPaciente, CancellationToken cancellationToken)
    {
        ValidarIdentificador(idPaciente, nameof(idPaciente));
        var asignaciones = await repository.ListarPorPacienteAsync(idPaciente, cancellationToken);
        return asignaciones.Select(Mapear).ToArray();
    }

    private static AsignacionPlanResponse Mapear(AsignacionPlan asignacion)
    {
        return new AsignacionPlanResponse(
            asignacion.Id,
            asignacion.IdPaciente,
            asignacion.IdPlan,
            asignacion.IdNutricionista,
            asignacion.FechaInicio,
            asignacion.FechaFin,
            asignacion.FechaAsignacionUtc);
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