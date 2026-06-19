using Microsoft.EntityFrameworkCore;
using NutriTec.Application.Abstractions.Persistence;
using NutriTec.Domain.Planes;
using NutriTec.Infrastructure.Sql.Persistence;
using NutriTec.Infrastructure.Sql.Persistence.Entities;

namespace NutriTec.Infrastructure.Sql.Repositories;

/*
 * Descripción:
 * Implementa la persistencia relacional de planes de alimentación mediante Entity
 * Framework Core y SQL Server.
 *
 * Entradas:
 * Recibe NutriTecDbContext y criterios definidos por Application.
 *
 * Salidas:
 * Inserta, consulta, actualiza y elimina planes junto con sus items.
 *
 * Restricciones:
 * No contiene reglas de negocio; mapea entre la entidad de dominio y la entidad SQL.
 */
public sealed class PlanSqlRepository(NutriTecDbContext context) : IPlanRepository
{
    public async Task<Plan> CrearAsync(Plan plan, CancellationToken cancellationToken)
    {
        var entidad = MapearAEntidad(plan);
        context.Planes.Add(entidad);
        await context.SaveChangesAsync(cancellationToken);
        return plan;
    }

    public async Task<Plan?> ObtenerPorIdAsync(Guid idPlan, CancellationToken cancellationToken)
    {
        var entidad = await context.Planes
            .AsNoTracking()
            .Include(plan => plan.Items)
            .SingleOrDefaultAsync(plan => plan.Id == idPlan, cancellationToken);

        return entidad is null ? null : MapearADominio(entidad);
    }

    public async Task<IReadOnlyCollection<Plan>> ListarPorNutricionistaAsync(string idNutricionista, CancellationToken cancellationToken)
    {
        var entidades = await context.Planes
            .AsNoTracking()
            .Include(plan => plan.Items)
            .Where(plan => plan.IdNutricionista == idNutricionista)
            .OrderBy(plan => plan.Nombre)
            .ToListAsync(cancellationToken);

        return entidades.Select(MapearADominio).ToArray();
    }

    public async Task<bool> ActualizarAsync(Plan plan, CancellationToken cancellationToken)
    {
        var entidad = await context.Planes
            .Include(p => p.Items)
            .SingleOrDefaultAsync(p => p.Id == plan.Id, cancellationToken);

        if (entidad is null)
        {
            return false;
        }

        entidad.Nombre = plan.Nombre;
        context.ItemsPlan.RemoveRange(entidad.Items);
        entidad.Items = plan.Items.Select(item => new ItemPlanSql
        {
            Id = item.Id,
            IdPlan = plan.Id,
            TiempoComida = (int)item.TiempoComida,
            IdProducto = item.IdProducto,
            Porciones = item.Porciones
        }).ToList();

        return await context.SaveChangesAsync(cancellationToken) > 0;
    }

    public async Task<bool> EliminarAsync(Guid idPlan, CancellationToken cancellationToken)
    {
        var entidad = await context.Planes.SingleOrDefaultAsync(plan => plan.Id == idPlan, cancellationToken);
        if (entidad is null)
        {
            return false;
        }

        context.Planes.Remove(entidad);
        return await context.SaveChangesAsync(cancellationToken) > 0;
    }

    public Task<bool> PertenecePlanAAsync(Guid idPlan, string idNutricionista, CancellationToken cancellationToken)
    {
        return context.Planes.AnyAsync(
            plan => plan.Id == idPlan && plan.IdNutricionista == idNutricionista,
            cancellationToken);
    }

    private static PlanSql MapearAEntidad(Plan plan) => new()
    {
        Id = plan.Id,
        IdNutricionista = plan.IdNutricionista,
        Nombre = plan.Nombre,
        FechaCreacionUtc = plan.FechaCreacionUtc,
        Items = plan.Items.Select(item => new ItemPlanSql
        {
            Id = item.Id,
            IdPlan = plan.Id,
            TiempoComida = (int)item.TiempoComida,
            IdProducto = item.IdProducto,
            Porciones = item.Porciones
        }).ToList()
    };

    private static Plan MapearADominio(PlanSql entidad) => new()
    {
        Id = entidad.Id,
        IdNutricionista = entidad.IdNutricionista,
        Nombre = entidad.Nombre,
        FechaCreacionUtc = entidad.FechaCreacionUtc,
        Items = entidad.Items.Select(item => new ItemPlan
        {
            Id = item.Id,
            IdPlan = item.IdPlan,
            TiempoComida = (TiempoComida)item.TiempoComida,
            IdProducto = item.IdProducto,
            Porciones = item.Porciones
        }).ToList()
    };
}