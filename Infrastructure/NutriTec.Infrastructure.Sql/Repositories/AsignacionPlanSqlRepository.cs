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
    /*
     * Descripción: Persiste la asignación de un plan a un paciente.
     * Entradas: Agregado y token de cancelación.
     * Salidas: Agregado persistido, con el identificador generado por la base de datos.
     * Restricciones: Recibe datos validados.
     */

    public async Task<AsignacionPlan> AsignarAsync(AsignacionPlan asignacion, CancellationToken cancellationToken)
    {
        var entidad = new AsignacionPlanSql
        {
            IdPlan = asignacion.IdPlan,
            IdUsuario = asignacion.IdPaciente,
            FechaInicio = asignacion.FechaInicio,
            FechaFin = asignacion.FechaFin
        };

        context.AsignacionesPlan.Add(entidad);
        await context.SaveChangesAsync(cancellationToken);

        return MapearADominio(entidad, asignacion.IdNutricionista);
    }

    /*
     * Descripción: Consulta la asignación de plan vigente para un paciente en la fecha actual.
     * Entradas: Identificador del paciente y token de cancelación.
     * Salidas: Asignación vigente o nula.
     * Restricciones: No modifica datos.
     */

    public async Task<AsignacionPlan?> ObtenerVigentePorPacienteAsync(int idPaciente, CancellationToken cancellationToken)
    {
        var hoy = DateOnly.FromDateTime(DateTime.UtcNow);

        var resultado = await (
            from asignacion in context.AsignacionesPlan.AsNoTracking()
            join plan in context.PlanesAlimentacion.AsNoTracking() on asignacion.IdPlan equals plan.IdPlan
            where asignacion.IdUsuario == idPaciente
                && asignacion.FechaInicio <= hoy
                && asignacion.FechaFin >= hoy
            orderby asignacion.IdAsignacion descending
            select new { asignacion, plan.CedulaNutricionista }
        ).FirstOrDefaultAsync(cancellationToken);

        return resultado is null ? null : MapearADominio(resultado.asignacion, resultado.CedulaNutricionista);
    }

    /*
     * Descripción: Lista el historial de asignaciones de un paciente.
     * Entradas: Identificador del paciente y token de cancelación.
     * Salidas: Colección de asignaciones ordenadas por fecha.
     * Restricciones: No modifica datos.
     */

    public async Task<IReadOnlyCollection<AsignacionPlan>> ListarPorPacienteAsync(int idPaciente, CancellationToken cancellationToken)
    {
        var resultados = await (
            from asignacion in context.AsignacionesPlan.AsNoTracking()
            join plan in context.PlanesAlimentacion.AsNoTracking() on asignacion.IdPlan equals plan.IdPlan
            where asignacion.IdUsuario == idPaciente
            orderby asignacion.FechaInicio descending
            select new { asignacion, plan.CedulaNutricionista }
        ).ToListAsync(cancellationToken);

        return resultados.Select(resultado => MapearADominio(resultado.asignacion, resultado.CedulaNutricionista)).ToArray();
    }

    private static AsignacionPlan MapearADominio(AsignacionPlanSql entidad, string cedulaNutricionista) => new()
    {
        Id = entidad.IdAsignacion,
        IdPaciente = entidad.IdUsuario,
        IdPlan = entidad.IdPlan,
        IdNutricionista = cedulaNutricionista,
        FechaInicio = entidad.FechaInicio,
        FechaFin = entidad.FechaFin,
        FechaAsignacionUtc = DateTime.UtcNow
    };
}