using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using NutriTec.Application.Abstractions.Persistence;
using NutriTec.Domain.Planes;
using NutriTec.Infrastructure.Sql.Persistence;
using NutriTec.Infrastructure.Sql.Persistence.Entities;

namespace NutriTec.Infrastructure.Sql.Repositories;

/*
 * Descripción:
 * Implementa la persistencia relacional de la asignación de planes de alimentación a
 * pacientes mediante SQL Server. La inserción utiliza el procedimiento almacenado
 * sp_AsignarPlanPaciente, que valida la asociación entre nutricionista y paciente,
 * evita duplicados y retorna el identificador generado.
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
     * Descripción: Persiste la asignación de un plan a un paciente usando el SP
     * sp_AsignarPlanPaciente, que valida que el paciente esté asociado al nutricionista
     * del plan y evita duplicados de asignación.
     * Entradas: Agregado y token de cancelación.
     * Salidas: Agregado persistido, con el identificador generado por el SP.
     * Restricciones: El SP lanza un error si el plan no existe, si el paciente no está
     * asociado al nutricionista, o si ya existe una asignación idéntica.
     */
    public async Task<AsignacionPlan> AsignarAsync(AsignacionPlan asignacion, CancellationToken cancellationToken)
    {
        var conexion = (SqlConnection)context.Database.GetDbConnection();
        if (conexion.State != System.Data.ConnectionState.Open)
            await conexion.OpenAsync(cancellationToken);

        await using var comando = new SqlCommand("sp_AsignarPlanPaciente", conexion)
        {
            CommandType = System.Data.CommandType.StoredProcedure
        };

        comando.Parameters.AddWithValue("@idPlan", asignacion.IdPlan);
        comando.Parameters.AddWithValue("@idUsuario", asignacion.IdPaciente);
        comando.Parameters.AddWithValue("@fechaInicio", asignacion.FechaInicio.ToDateTime(TimeOnly.MinValue));
        comando.Parameters.AddWithValue("@fechaFin", asignacion.FechaFin.ToDateTime(TimeOnly.MinValue));

        var resultado = await comando.ExecuteScalarAsync(cancellationToken);
        var idAsignacion = Convert.ToInt32(resultado);

        return new AsignacionPlan
        {
            Id = idAsignacion,
            IdPaciente = asignacion.IdPaciente,
            IdPlan = asignacion.IdPlan,
            IdNutricionista = asignacion.IdNutricionista,
            FechaInicio = asignacion.FechaInicio,
            FechaFin = asignacion.FechaFin,
            FechaAsignacionUtc = DateTime.UtcNow
        };
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