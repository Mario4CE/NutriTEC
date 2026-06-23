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
 * Devuelve DTOs preparados para la API, incluyendo el total calórico calculado por la
 * base de datos mediante función y trigger SQL.
 *
 * Restricciones:
 * Rechaza nombres vacíos, planes sin items y productos inexistentes.
 */

public sealed class PlanService(
    IPlanRepository repository,
    IProductoRepository productoRepository) : IPlanService
{
    /*
     * Descripción: Crea un plan de alimentación para un nutricionista.
     * Entradas: Cédula del nutricionista, DTO de creación y token de cancelación.
     * Salidas: Devuelve el plan creado como DTO, con el total calórico calculado por SQL.
     * Restricciones: El nombre y al menos un item son obligatorios; los productos deben existir.
     */

    public async Task<PlanResponse> CrearAsync(
        string idNutricionista,
        CrearPlanRequest request,
        CancellationToken cancellationToken)
    {
        ValidarTexto(idNutricionista, nameof(idNutricionista));
        ValidarTexto(request.Nombre, nameof(request.Nombre));
        ValidarItems(request.Items);

        await ValidarProductosExistenAsync(request.Items, cancellationToken);

        var plan = new Plan
        {
            IdNutricionista = idNutricionista,
            Nombre = request.Nombre.Trim(),
            FechaCreacionUtc = DateTime.UtcNow,
            Items = request.Items.Select(item => new ItemPlan
            {
                TiempoComida = (Domain.Planes.TiempoComida)item.TiempoComida,
                IdProducto = item.IdProducto,
                Porciones = item.Porciones
            }).ToList()
        };

        var creado = await repository.CrearAsync(plan, cancellationToken);
        var productos = await ObtenerProductosDelPlanAsync(creado, cancellationToken);
        return Mapear(creado, productos);
    }

    /*
     * Descripción: Consulta un plan por identificador.
     * Entradas: Identificador del plan y token de cancelación.
     * Salidas: Devuelve el DTO encontrado o nulo.
     * Restricciones: El identificador no puede ser vacío.
     */

    public async Task<PlanResponse?> ObtenerPorIdAsync(int idPlan, CancellationToken cancellationToken)
    {
        var plan = await repository.ObtenerPorIdAsync(idPlan, cancellationToken);
        if (plan is null)
        {
            return null;
        }

        var productos = await ObtenerProductosDelPlanAsync(plan, cancellationToken);
        return Mapear(plan, productos);
    }

    /*
     * Descripción: Lista los planes creados por un nutricionista.
     * Entradas: Cédula del nutricionista y token de cancelación.
     * Salidas: Devuelve una colección de DTOs.
     * Restricciones: La cédula no puede estar vacía.
     */

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

    /*
     * Descripción: Actualiza el nombre y los items de un plan existente.
     * Entradas: Cédula del nutricionista, identificador del plan, DTO de edición y cancelación.
     * Salidas: Devuelve verdadero cuando el plan existe, pertenece al nutricionista y fue actualizado.
     * Restricciones: El plan debe pertenecer al nutricionista que solicita la edición.
     */

    public async Task<bool> ActualizarAsync(
        string idNutricionista,
        int idPlan,
        ActualizarPlanRequest request,
        CancellationToken cancellationToken)
    {
        ValidarTexto(idNutricionista, nameof(idNutricionista));
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
                IdPlan = idPlan,
                TiempoComida = (Domain.Planes.TiempoComida)item.TiempoComida,
                IdProducto = item.IdProducto,
                Porciones = item.Porciones
            }).ToList()
        };

        return await repository.ActualizarAsync(plan, cancellationToken);
    }

    /*
     * Descripción: Elimina un plan de alimentación.
     * Entradas: Cédula del nutricionista, identificador del plan y token de cancelación.
     * Salidas: Devuelve verdadero cuando el plan existe, pertenece al nutricionista y fue eliminado.
     * Restricciones: El plan debe pertenecer al nutricionista que solicita la eliminación.
     */

    public async Task<bool> EliminarAsync(string idNutricionista, int idPlan, CancellationToken cancellationToken)
    {
        ValidarTexto(idNutricionista, nameof(idNutricionista));

        if (!await repository.PertenecePlanAAsync(idPlan, idNutricionista, cancellationToken))
        {
            throw new ConflictoException("El plan no existe o no pertenece a este nutricionista.");
        }

        return await repository.EliminarAsync(idPlan, cancellationToken);
    }

    private async Task ValidarProductosExistenAsync(
        IReadOnlyCollection<ItemPlanRequest> items,
        CancellationToken cancellationToken)
    {
        foreach (var idProducto in items.Select(item => item.IdProducto).Distinct())
        {
            var producto = await productoRepository.ObtenerPorIdAsync(idProducto, cancellationToken);
            if (producto is null)
            {
                throw new ArgumentException($"El producto {idProducto} no existe.", nameof(items));
            }
        }
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

    private static void ValidarTexto(string texto, string nombreParametro)
    {
        if (string.IsNullOrWhiteSpace(texto))
        {
            throw new ArgumentException("El texto no puede estar vacío.", nombreParametro);
        }
    }
}