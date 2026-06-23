using NutriTec.Application.Abstractions.Persistence;
using NutriTec.Application.Abstractions.Services;
using NutriTec.Application.Common;
using NutriTec.Contracts.Planes;
using NutriTec.Domain.Planes;

namespace NutriTec.Application.Planes;

/*
 * Descripción:
 * Implementa las reglas de aplicación para asignar un plan de alimentación a un
 * paciente durante un periodo de tiempo determinado.
 *
 * Entradas:
 * Recibe DTOs de solicitud y repositorios desacoplados de persistencia.
 *
 * Salidas:
 * Devuelve DTOs preparados para la API o confirma modificaciones.
 *
 * Restricciones:
 * El paciente debe estar asociado al nutricionista y el plan debe pertenecerle;
 * la fecha de inicio no puede ser posterior a la fecha de fin.
 */

public sealed class AsignacionPlanService(
    IAsignacionPlanRepository repository,
    IPlanRepository planRepository,
    IPacienteRepository pacienteRepository) : IAsignacionPlanService
{
    /*
     * Descripción: Asigna un plan a un paciente para un periodo de tiempo específico.
     * Entradas: Cédula del nutricionista, DTO de asignación y token de cancelación.
     * Salidas: Devuelve la asignación creada como DTO.
     * Restricciones: El paciente debe estar asociado al nutricionista y el plan debe pertenecerle.
     */

    public async Task<AsignacionPlanResponse> AsignarAsync(
        string idNutricionista,
        AsignarPlanRequest request,
        CancellationToken cancellationToken)
    {
        ValidarTexto(idNutricionista, nameof(idNutricionista));

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

    /*
     * Descripción: Consulta la asignación de plan vigente para un paciente.
     * Entradas: Identificador del paciente y token de cancelación.
     * Salidas: Devuelve la asignación vigente como DTO, o nulo si no hay ninguna.
     * Restricciones: El identificador no puede ser vacío.
     */

    public async Task<AsignacionPlanResponse?> ObtenerVigentePorPacienteAsync(int idPaciente, CancellationToken cancellationToken)
    {
        var asignacion = await repository.ObtenerVigentePorPacienteAsync(idPaciente, cancellationToken);
        return asignacion is null ? null : Mapear(asignacion);
    }

    /*
     * Descripción: Lista el historial de asignaciones de un paciente.
     * Entradas: Identificador del paciente y token de cancelación.
     * Salidas: Devuelve una colección de DTOs.
     * Restricciones: El identificador no puede ser vacío.
     */

    public async Task<IReadOnlyCollection<AsignacionPlanResponse>> ListarPorPacienteAsync(int idPaciente, CancellationToken cancellationToken)
    {
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

    private static void ValidarTexto(string texto, string nombreParametro)
    {
        if (string.IsNullOrWhiteSpace(texto))
        {
            throw new ArgumentException("El texto no puede estar vacío.", nombreParametro);
        }
    }
}