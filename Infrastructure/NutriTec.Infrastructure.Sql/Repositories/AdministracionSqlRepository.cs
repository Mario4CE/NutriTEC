using Microsoft.EntityFrameworkCore;
using NutriTec.Application.Abstractions.Persistence;
using NutriTec.Domain.Productos;
using NutriTec.Infrastructure.Sql.Persistence;

namespace NutriTec.Infrastructure.Sql.Repositories;

/*
 * Repositorio SQL dedicado a operaciones administrativas.
 * Mantiene ListarProductosPendientesAsync y AprobarProductoAsync fuera de ProductoCrudSqlRepository
 * para que no existan miembros duplicados en el repositorio de productos.
 */
public sealed class AdministracionSqlRepository(NutriTecDbContext context) : IAdministracionRepository
{
    public async Task<IReadOnlyCollection<Producto>> ListarProductosPendientesAsync(CancellationToken cancellationToken)
    {
        return await context.Productos
            .AsNoTracking()
            .Where(producto => !producto.EstaAprobado)
            .OrderBy(producto => producto.FechaCreacionUtc)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> AprobarProductoAsync(Guid idProducto, CancellationToken cancellationToken)
    {
        var productosActualizados = await context.Productos
            .Where(producto => producto.Id == idProducto && !producto.EstaAprobado)
            .ExecuteUpdateAsync(
                setters => setters.SetProperty(producto => producto.EstaAprobado, true),
                cancellationToken);

        return productosActualizados == 1;
    }
}
