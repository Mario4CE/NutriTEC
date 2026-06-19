using Microsoft.EntityFrameworkCore;
using NutriTec.Application.Abstractions.Persistence;
using NutriTec.Domain.Pacientes;
using NutriTec.Infrastructure.Sql.Persistence;
using NutriTec.Infrastructure.Sql.Persistence.Entities;

namespace NutriTec.Infrastructure.Sql.Repositories;

/*
 * Descripción:
 * Implementa la persistencia relacional de la asociación entre nutricionistas y pacientes
 * mediante Entity Framework Core y SQL Server.
 *
 * Entradas:
 * Recibe NutriTecDbContext y criterios definidos por Application.
 *
 * Salidas:
 * Inserta, consulta y elimina asociaciones entre nutricionistas y pacientes.
 *
 * Restricciones:
 * No contiene reglas de negocio; mapea entre la entidad de dominio y la entidad SQL.
 */
public sealed class PacienteSqlRepository(NutriTecDbContext context) : IPacienteRepository
{
    public async Task<PacienteNutricionista> AsociarAsync(PacienteNutricionista asociacion, CancellationToken cancellationToken)
    {
        var entidad = MapearAEntidad(asociacion);
        context.PacientesNutricionista.Add(entidad);
        await context.SaveChangesAsync(cancellationToken);
        return asociacion;
    }

    public Task<bool> ExisteAsociacionAsync(string idNutricionista, Guid idPaciente, CancellationToken cancellationToken)
    {
        return context.PacientesNutricionista.AnyAsync(
            asociacion => asociacion.IdNutricionista == idNutricionista && asociacion.IdPaciente == idPaciente,
            cancellationToken);
    }

    public async Task<IReadOnlyCollection<PacienteNutricionista>> ListarPorNutricionistaAsync(string idNutricionista, CancellationToken cancellationToken)
    {
        var entidades = await context.PacientesNutricionista
            .AsNoTracking()
            .Where(asociacion => asociacion.IdNutricionista == idNutricionista)
            .OrderBy(asociacion => asociacion.FechaAsociacionUtc)
            .ToListAsync(cancellationToken);

        return entidades.Select(MapearADominio).ToArray();
    }

    public async Task<PacienteNutricionista?> ObtenerPorIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var entidad = await context.PacientesNutricionista
            .AsNoTracking()
            .SingleOrDefaultAsync(asociacion => asociacion.Id == id, cancellationToken);

        return entidad is null ? null : MapearADominio(entidad);
    }

    public Task<bool> EsPacienteDeAsync(string idNutricionista, Guid idPaciente, CancellationToken cancellationToken)
    {
        return context.PacientesNutricionista.AnyAsync(
            asociacion => asociacion.IdNutricionista == idNutricionista && asociacion.IdPaciente == idPaciente,
            cancellationToken);
    }

    public async Task<bool> DesasociarAsync(Guid id, CancellationToken cancellationToken)
    {
        var entidad = await context.PacientesNutricionista.SingleOrDefaultAsync(asociacion => asociacion.Id == id, cancellationToken);
        if (entidad is null)
        {
            return false;
        }

        context.PacientesNutricionista.Remove(entidad);
        return await context.SaveChangesAsync(cancellationToken) == 1;
    }

    private static PacienteNutricionistaSql MapearAEntidad(PacienteNutricionista asociacion) => new()
    {
        Id = asociacion.Id,
        IdNutricionista = asociacion.IdNutricionista,
        IdPaciente = asociacion.IdPaciente,
        FechaAsociacionUtc = asociacion.FechaAsociacionUtc
    };

    private static PacienteNutricionista MapearADominio(PacienteNutricionistaSql entidad) => new()
    {
        Id = entidad.Id,
        IdNutricionista = entidad.IdNutricionista,
        IdPaciente = entidad.IdPaciente,
        FechaAsociacionUtc = entidad.FechaAsociacionUtc
    };
}