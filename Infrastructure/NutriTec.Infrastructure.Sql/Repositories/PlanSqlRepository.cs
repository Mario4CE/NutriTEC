using Microsoft.EntityFrameworkCore;
using NutriTec.Application.Abstractions.Persistence;
using NutriTec.Domain.Planes;
using NutriTec.Infrastructure.Sql.Persistence;
using NutriTec.Infrastructure.Sql.Persistence.Entities;

namespace NutriTec.Infrastructure.Sql.Repositories;

/*
 * Descripción:
 * Implementa la persistencia relacional de planes de alimentación mediante Entity
 * Framework Core y SQL Server, combinando las tablas PLAN_ALIMENTACION,
 * TIEMPO_COMIDA_PLAN y PLAN_PRODUCTO.
 *
 * Entradas:
 * Recibe NutriTecDbContext y criterios definidos por Application.
 *
 * Salidas:
 * Inserta, consulta, actualiza y elimina planes junto con sus tiempos de comida e items.
 *
 * Restricciones:
 * No contiene reglas de negocio; total_calorias se recalcula automáticamente mediante el
 * trigger trg_RecalcularTotalPlan, por lo que esta capa no lo escribe manualmente.
 */
public sealed class PlanSqlRepository(NutriTecDbContext context) : IPlanRepository
{
    private static readonly string[] NombresTiempoComida =
    [
        "Desayuno",
        "Merienda Mañana",
        "Almuerzo",
        "Merienda Tarde",
        "Cena"
    ];

    /*
     * Descripción: Persiste un plan con sus tiempos de comida e items.
     * Entradas: Agregado y token de cancelación.
     * Salidas: Agregado persistido, con el identificador generado por la base de datos.
     * Restricciones: Recibe datos validados.
     */

    public async Task<Plan> CrearAsync(Plan plan, CancellationToken cancellationToken)
    {
        var entidad = new PlanAlimentacionSql
        {
            Nombre = plan.Nombre,
            CedulaNutricionista = plan.IdNutricionista,
            Tiempos = AgruparPorTiempo(plan.Items)
        };

        context.PlanesAlimentacion.Add(entidad);
        await context.SaveChangesAsync(cancellationToken);

        return MapearADominio(entidad);
    }

    /*
     * Descripción: Consulta un plan por identificador, incluyendo sus tiempos e items.
     * Entradas: Identificador y token de cancelación.
     * Salidas: Plan o nulo.
     * Restricciones: No modifica datos.
     */

    public async Task<Plan?> ObtenerPorIdAsync(int idPlan, CancellationToken cancellationToken)
    {
        var entidad = await context.PlanesAlimentacion
            .AsNoTracking()
            .Include(plan => plan.Tiempos)
            .ThenInclude(tiempo => tiempo.Productos)
            .SingleOrDefaultAsync(plan => plan.IdPlan == idPlan, cancellationToken);

        return entidad is null ? null : MapearADominio(entidad);
    }

    /*
     * Descripción: Lista los planes creados por un nutricionista.
     * Entradas: Cédula del nutricionista y token de cancelación.
     * Salidas: Colección de planes.
     * Restricciones: No modifica datos.
     */

    public async Task<IReadOnlyCollection<Plan>> ListarPorNutricionistaAsync(string idNutricionista, CancellationToken cancellationToken)
    {
        var entidades = await context.PlanesAlimentacion
            .AsNoTracking()
            .Include(plan => plan.Tiempos)
            .ThenInclude(tiempo => tiempo.Productos)
            .Where(plan => plan.CedulaNutricionista == idNutricionista)
            .OrderBy(plan => plan.Nombre)
            .ToListAsync(cancellationToken);

        return entidades.Select(MapearADominio).ToArray();
    }

    /*
     * Descripción: Reemplaza el nombre y los tiempos/items de un plan existente.
     * Entradas: Agregado con los datos actualizados y token de cancelación.
     * Salidas: Confirmación.
     * Restricciones: Reemplaza la colección completa de tiempos e items.
     */

    public async Task<bool> ActualizarAsync(Plan plan, CancellationToken cancellationToken)
    {
        var entidad = await context.PlanesAlimentacion
            .Include(p => p.Tiempos)
            .ThenInclude(t => t.Productos)
            .SingleOrDefaultAsync(p => p.IdPlan == plan.Id, cancellationToken);

        if (entidad is null)
        {
            return false;
        }

        entidad.Nombre = plan.Nombre;
        context.TiemposComidaPlan.RemoveRange(entidad.Tiempos);
        entidad.Tiempos = AgruparPorTiempo(plan.Items);

        return await context.SaveChangesAsync(cancellationToken) > 0;
    }

    /*
     * Descripción: Elimina un plan por identificador.
     * Entradas: Identificador y token de cancelación.
     * Salidas: Confirmación.
     * Restricciones: Devuelve falso si no existe.
     */

    public async Task<bool> EliminarAsync(int idPlan, CancellationToken cancellationToken)
    {
        var entidad = await context.PlanesAlimentacion.SingleOrDefaultAsync(plan => plan.IdPlan == idPlan, cancellationToken);
        if (entidad is null)
        {
            return false;
        }

        context.PlanesAlimentacion.Remove(entidad);
        return await context.SaveChangesAsync(cancellationToken) > 0;
    }

    /*
     * Descripción: Verifica que un plan pertenezca a un nutricionista específico.
     * Entradas: Identificador del plan, cédula del nutricionista y token de cancelación.
     * Salidas: Verdadero si el plan existe y pertenece a ese nutricionista.
     * Restricciones: No modifica datos.
     */

    public Task<bool> PertenecePlanAAsync(int idPlan, string idNutricionista, CancellationToken cancellationToken)
    {
        return context.PlanesAlimentacion.AnyAsync(
            plan => plan.IdPlan == idPlan && plan.CedulaNutricionista == idNutricionista,
            cancellationToken);
    }

    private static List<TiempoComidaPlanSql> AgruparPorTiempo(IReadOnlyCollection<ItemPlan> items)
    {
        return items
            .GroupBy(item => item.TiempoComida)
            .Select(grupo => new TiempoComidaPlanSql
            {
                TipoComida = NombresTiempoComida[(int)grupo.Key - 1],
                Productos = grupo.Select(item => new PlanProductoSql
                {
                    IdProducto = item.IdProducto,
                    CantidadPorciones = item.Porciones
                }).ToList()
            })
            .ToList();
    }

    private static Plan MapearADominio(PlanAlimentacionSql entidad)
    {
        var items = entidad.Tiempos.SelectMany(tiempo => tiempo.Productos.Select(producto => new ItemPlan
        {
            IdPlan = entidad.IdPlan,
            TiempoComida = (TiempoComida)(Array.IndexOf(NombresTiempoComida, tiempo.TipoComida) + 1),
            IdProducto = producto.IdProducto,
            Porciones = producto.CantidadPorciones
        })).ToList();

        return new Plan
        {
            Id = entidad.IdPlan,
            IdNutricionista = entidad.CedulaNutricionista,
            Nombre = entidad.Nombre,
            FechaCreacionUtc = DateTime.UtcNow,
            Items = items
        };
    }
}