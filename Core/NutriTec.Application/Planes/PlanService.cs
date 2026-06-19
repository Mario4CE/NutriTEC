using NutriTec.Application.Abstractions.Persistence;
using NutriTec.Application.Abstractions.Services;
using NutriTec.Application.Common;
using NutriTec.Contracts.Planes;
using NutriTec.Domain.Planes;

namespace NutriTec.Application.Planes;

/*
 * Descripción:
 * Implementa las reglas de aplicación para crear, consultar, actualizar y eliminar
 * planes de alimentación compuestos por productos distribuidos en cinco tiempos de comida.
 *
 * Entradas:
 * Recibe DTOs de solicitud y repositorios desacoplados de persistencia.
 *
 * Salidas:
 * Devuelve DTOs preparados para la API, incluyendo el total calórico calculado.
 *
 * Restricciones:
 * Rechaza nombres vacíos, planes sin items y productos inexistentes; el total calórico
 * se calcula dinámicamente a partir de los productos referenciados.
 */

public sealed class PlanService(
    IPlanRepository repository,
    IProductoRepository productoRepository) : IPlanService
{
    public async Task<PlanResponse> CrearAsync(
        string idNutricionista,
        CrearPlanRequest request,
        CancellationToken cancellationToken)
    {
        ValidarTexto(idNutricionista, nameof(idNutricionista));
        ValidarTexto(request.Nombre, nameof(request.Nombre));
        ValidarItems(request.Items);

        var productos = await ValidarProductosExistenAsync(request.Items, cancellationToken);

        var plan = new Plan
        {
            Id = Guid.NewGuid(),
            IdNutricionista = idNutricionista,
            Nombre = request.Nombre.Trim(),
            FechaCreacionUtc = DateTime.UtcNow,
            Items = request.Items.Select(item => new ItemPlan
            {
                Id = Guid.NewGuid(),
                TiempoComida = (Domain.Planes.TiempoComida)item.TiempoComida,
                IdProducto = item.IdProducto,
                Porciones = item.Porciones
            }).ToList()
        };

        var creado = await repository.CrearAsync(plan, cancellationToken);
        return Mapear(creado, productos);
    }

    public async Task<PlanResponse?> ObtenerPorIdAsync(Guid idPlan, CancellationToken cancellationToken)
    {
        ValidarIdentificador(idPlan, nameof(idPlan));
        var plan = await repository.ObtenerPorIdAsync(idPlan, cancellationToken);
        if (plan is null)
        {
            return null;
        }

        var productos = await ObtenerProductosDelPlanAsync(plan, cancellationToken);
        return Mapear(plan, productos);
    }

    public async Task<IReadOnlyCollection<PlanResponse>> ListarPorNutricionistaAsync(
        string idNutricionista,
        CancellationToken cancellationToken)
    {
        ValidarTexto(idNutricionista, nameof(idNutricionista));
        var planes = await repository.ListarPorNutricionistaAsync(idNutricionista, cancellationToken);

        var respuestas = new List<PlanResponse>(planes.Count);
        foreach (var plan in planes)
        {
            var productos = await ObtenerProductosDelPlanAsync(plan, cancellationToken);
            respuestas.Add(Mapear(plan, productos));
        }

        return respuestas;
    }

    public async Task<bool> ActualizarAsync(
        string idNutricionista,
        Guid idPlan,
        ActualizarPlanRequest request,
        CancellationToken cancellationToken)
    {
        ValidarTexto(idNutricionista, nameof(idNutricionista));
        ValidarIdentificador(idPlan, nameof(idPlan));
        ValidarTexto(request.Nombre, nameof(request.Nombre));
        ValidarItems(request.Items);

        if (!await repository.PertenecePlanAAsync(idPlan, idNutricionista, cancellationToken))
        {
            throw new ConflictoException("El plan no existe o no pertenece a este nutricionista.");
        }

        await ValidarProductosExistenAsync(request.Items, cancellationToken);

        var plan = new Plan
        {
            Id = idPlan,
            IdNutricionista = idNutricionista,
            Nombre = request.Nombre.Trim(),
            FechaCreacionUtc = DateTime.UtcNow,
            Items = request.Items.Select(item => new ItemPlan
            {
                Id = Guid.NewGuid(),
                IdPlan = idPlan,
                TiempoComida = (Domain.Planes.TiempoComida)item.TiempoComida,
                IdProducto = item.IdProducto,
                Porciones = item.Porciones
            }).ToList()
        };

        return await repository.ActualizarAsync(plan, cancellationToken);
    }

    public async Task<bool> EliminarAsync(string idNutricionista, Guid idPlan, CancellationToken cancellationToken)
    {
        ValidarTexto(idNutricionista, nameof(idNutricionista));
        ValidarIdentificador(idPlan, nameof(idPlan));

        if (!await repository.PertenecePlanAAsync(idPlan, idNutricionista, cancellationToken))
        {
            throw new ConflictoException("El plan no existe o no pertenece a este nutricionista.");
        }

        return await repository.EliminarAsync(idPlan, cancellationToken);
    }

    private async Task<Dictionary<Guid, Domain.Productos.Producto>> ValidarProductosExistenAsync(
        IReadOnlyCollection<ItemPlanRequest> items,
        CancellationToken cancellationToken)
    {
        var productos = new Dictionary<Guid, Domain.Productos.Producto>();
        foreach (var idProducto in items.Select(item => item.IdProducto).Distinct())
        {
            var producto = await productoRepository.ObtenerPorIdAsync(idProducto, cancellationToken);
            if (producto is null)
            {
                throw new ArgumentException($"El producto {idProducto} no existe.", nameof(items));
            }

            productos[idProducto] = producto;
        }

        return productos;
    }

    private async Task<Dictionary<Guid, Domain.Productos.Producto>> ObtenerProductosDelPlanAsync(
        Plan plan,
        CancellationToken cancellationToken)
    {
        var productos = new Dictionary<Guid, Domain.Productos.Producto>();
        foreach (var idProducto in plan.Items.Select(item => item.IdProducto).Distinct())
        {
            var producto = await productoRepository.ObtenerPorIdAsync(idProducto, cancellationToken);
            if (producto is not null)
            {
                productos[idProducto] = producto;
            }
        }

        return productos;
    }

    private static PlanResponse Mapear(Plan plan, IReadOnlyDictionary<Guid, Domain.Productos.Producto> productos)
    {
        var itemsResponse = plan.Items.Select(item =>
        {
            productos.TryGetValue(item.IdProducto, out var producto);
            var calorias = (producto?.Calorias ?? 0) * item.Porciones;

            return new ItemPlanResponse(
                item.Id,
                (Contracts.Planes.TiempoComida)item.TiempoComida,
                item.IdProducto,
                producto?.Nombre ?? string.Empty,
                item.Porciones,
                calorias);
        }).ToArray();

        var caloriasTotales = itemsResponse.Sum(item => item.CaloriasAportadas);

        return new PlanResponse(
            plan.Id,
            plan.IdNutricionista,
            plan.Nombre,
            plan.FechaCreacionUtc,
            itemsResponse,
            caloriasTotales);
    }

    private static void ValidarItems(IReadOnlyCollection<ItemPlanRequest> items)
    {
        if (items is null || items.Count == 0)
        {
            throw new ArgumentException("El plan debe incluir al menos un producto.", nameof(items));
        }

        if (items.Any(item => item.Porciones <= 0))
        {
            throw new ArgumentException("Las porciones de cada item deben ser mayores que cero.", nameof(items));
        }
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