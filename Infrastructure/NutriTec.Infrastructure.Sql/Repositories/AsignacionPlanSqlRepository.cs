using Microsoft.EntityFrameworkCore;
using NutriTec.Application.Abstractions.Persistence;
using NutriTec.Domain.Planes;
using NutriTec.Infrastructure.Sql.Persistence;
using NutriTec.Infrastructure.Sql.Persistence.Entities;

namespace NutriTec.Infrastructure.Sql.Repositories;

/*
 * Descripción:
 * Implementa la persistencia relacional de la asignación de planes de alimentación a
 * pacientes mediante Entity Framework Core y SQL Server.
 *
 * Entradas:
 * Recibe NutriTecDbContext y criterios definidos por Application.
 *
 * Salidas:
 * Inserta y consulta asignaciones de planes a pacientes.
 *
 * Restricciones:
 * No contiene reglas de negocio; mapea entre la entidad de dominio y la entidad SQL.
 */
public sealed class AsignacionPlanSqlRepository(NutriTecDbContext context) : IAsignacionPlanRepository
{
    public async Task<AsignacionPlan> AsignarAsync(AsignacionPlan asignacion, CancellationToken cancellationToken)
    {
        var entidad = MapearAEntidad(asignacion);
        context.AsignacionesPlan.Add(entidad);
        await context.SaveChangesAsync(cancellationToken);
        return asignacion;
    }

    public async Task<AsignacionPlan?> ObtenerVigentePorPacienteAsync(Guid idPaciente, CancellationToken cancellationToken)
    {
        var hoy = DateOnly.FromDateTime(DateTime.UtcNow);

        var entidad = await context.AsignacionesPlan
            .AsNoTracking()
            .Where(asignacion => asignacion.IdPaciente == idPaciente
                && asignacion.FechaInicio <= hoy
                && asignacion.FechaFin >= hoy)
            .OrderByDescending(asignacion => asignacion.FechaAsignacionUtc)
            .FirstOrDefaultAsync(cancellationToken);

        return entidad is null ? null : MapearADominio(entidad);
    }

    public async Task<IReadOnlyCollection<AsignacionPlan>> ListarPorPacienteAsync(Guid idPaciente, CancellationToken cancellationToken)
    {
        var entidades = await context.AsignacionesPlan
            .AsNoTracking()
            .Where(asignacion => asignacion.IdPaciente == idPaciente)
            .OrderByDescending(asignacion => asignacion.FechaInicio)
            .ToListAsync(cancellationToken);

        return entidades.Select(MapearADominio).ToArray();
    }

    private static AsignacionPlanSql MapearAEntidad(AsignacionPlan asignacion) => new()
    {
        Id = asignacion.Id,
        IdPaciente = asignacion.IdPaciente,
        IdPlan = asignacion.IdPlan,
        IdNutricionista = asignacion.IdNutricionista,
        FechaInicio = asignacion.FechaInicio,
        FechaFin = asignacion.FechaFin,
        FechaAsignacionUtc = asignacion.FechaAsignacionUtc
    };

    private static AsignacionPlan MapearADominio(AsignacionPlanSql entidad) => new()
    {
        Id = entidad.Id,
        IdPaciente = entidad.IdPaciente,
        IdPlan = entidad.IdPlan,
        IdNutricionista = entidad.IdNutricionista,
        FechaInicio = entidad.FechaInicio,
        FechaFin = entidad.FechaFin,
        FechaAsignacionUtc = entidad.FechaAsignacionUtc
    };
}