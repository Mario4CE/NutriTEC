using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using NutriTec.Application.Abstractions.Persistence;
using NutriTec.Domain.Productos;
using NutriTec.Infrastructure.Sql.Persistence;

namespace NutriTec.Infrastructure.Sql.Repositories;

/*
 * Descripción:
 * Implementa la persistencia relacional de productos alimenticios mediante Entity Framework Core y SQL Server.
 *
 * Entradas:
 * Recibe NutriTecDbContext y criterios definidos por Application.
 *
 * Salidas:
 * Inserta, consulta, actualiza y elimina productos.
 *
 * Restricciones:
 * No contiene reglas de negocio; utiliza consultas sin seguimiento cuando no modificará entidades.
 * Las operaciones administrativas se implementan en AdministracionSqlRepository para evitar duplicar métodos de aprobación.
 */
public sealed class ProductoSqlRepository(NutriTecDbContext context) : IProductoRepository, IAdministracionRepository
{
    /*
     * Descripción:
     * Inserta un producto.
     * Entradas:
     * Recibe agregado y token de cancelación.
     * Salidas:
     * Devuelve el agregado persistido.
     * Restricciones:
     * La capa de aplicación valida sus datos.
     */
    public async Task<Producto> CrearAsync(Producto producto, CancellationToken cancellationToken)
    {
        context.Productos.Add(producto);
        await context.SaveChangesAsync(cancellationToken);
        return producto;
    }

    /*
     * Descripción:
     * Consulta un producto por identificador.
     * Entradas:
     * Recibe identificador y token de cancelación.
     * Salidas:
     * Devuelve el agregado encontrado o nulo.
     * Restricciones:
     * No aplica reglas de negocio.
     */
    public Task<Producto?> ObtenerPorIdAsync(Guid idProducto, CancellationToken cancellationToken)
    {
        return context.Productos.SingleOrDefaultAsync(producto => producto.Id == idProducto, cancellationToken);
    }

    /*
     * Descripción:
     * Lista productos ordenados por nombre.
     * Entradas:
     * Recibe token de cancelación.
     * Salidas:
     * Devuelve agregados sin seguimiento.
     * Restricciones:
     * No modifica datos.
     */
    public async Task<IReadOnlyCollection<Producto>> ListarAsync(CancellationToken cancellationToken)
    {
        return await context.Productos.AsNoTracking().OrderBy(producto => producto.Nombre).ToListAsync(cancellationToken);
    }

    /*
     * Descripción:
     * Busca productos por coincidencia de nombre.
     * Entradas:
     * Recibe criterio y token de cancelación.
     * Salidas:
     * Devuelve agregados coincidentes sin seguimiento.
     * Restricciones:
     * La colación configurada en SQL Server determina sensibilidad a mayúsculas.
     */
    public async Task<IReadOnlyCollection<Producto>> BuscarPorNombreAsync(string nombre, CancellationToken cancellationToken)
    {
        return await context.Productos
            .AsNoTracking()
            .Where(producto => producto.Nombre.Contains(nombre))
            .OrderBy(producto => producto.Nombre)
            .ToListAsync(cancellationToken);
    }

    /*
     * Descripción:
     * Consulta un producto por código de barras.
     * Entradas:
     * Recibe código y token de cancelación.
     * Salidas:
     * Devuelve el agregado encontrado o nulo.
     * Restricciones:
     * No modifica datos.
     */
    public Task<Producto?> ObtenerPorCodigoBarrasAsync(string codigoBarras, CancellationToken cancellationToken)
    {
        return context.Productos.AsNoTracking().SingleOrDefaultAsync(producto => producto.CodigoBarras == codigoBarras, cancellationToken);
    }

    /*
     * Descripción:
     * Verifica si un código de barras ya está asignado.
     * Entradas:
     * Recibe código, identificador opcional excluido y token de cancelación.
     * Salidas:
     * Devuelve verdadero cuando existe una coincidencia.
     * Restricciones:
     * Permite excluir el producto editado.
     */
    public Task<bool> ExisteCodigoBarrasAsync(string codigoBarras, Guid? idProductoExcluido, CancellationToken cancellationToken)
    {
        return context.Productos.AnyAsync(
            producto => producto.CodigoBarras == codigoBarras && producto.Id != idProductoExcluido,
            cancellationToken);
    }

    /*
     * Descripción:
     * Persiste cambios sobre un producto existente.
     * Entradas:
     * Recibe agregado y token de cancelación.
     * Salidas:
     * Devuelve verdadero cuando se guardó una modificación.
     * Restricciones:
     * No aplica reglas de negocio.
     */
    public async Task<bool> ActualizarAsync(Producto producto, CancellationToken cancellationToken)
    {
        context.Productos.Update(producto);
        return await context.SaveChangesAsync(cancellationToken) == 1;
    }

    /*
     * Descripción:
     * Lista productos que todavía requieren aprobación administrativa.
     * Entradas:
     * Recibe token de cancelación.
     * Salidas:
     * Devuelve productos pendientes ordenados por fecha de creación.
     * Restricciones:
     * Utiliza una consulta sin seguimiento y no modifica datos.
     */
    public async Task<IReadOnlyCollection<Producto>> ListarPendientesAsync(CancellationToken cancellationToken)
    {
        return await context.Productos
            .AsNoTracking()
            .Where(producto => !producto.EstaAprobado)
            .OrderBy(producto => producto.FechaCreacionUtc)
            .ToListAsync(cancellationToken);
    }

    public Task<IReadOnlyCollection<Producto>> ListarProductosPendientesAsync(CancellationToken cancellationToken)
    {
        return ListarPendientesAsync(cancellationToken);
    }

    /*
     * Descripción:
     * Cambia atómicamente un producto pendiente al estado aprobado.
     * Entradas:
     * Recibe identificador del producto y token de cancelación.
     * Salidas:
     * Devuelve verdadero cuando actualiza exactamente un producto pendiente.
     * Restricciones:
     * No vuelve a escribir productos aprobados y evita cargar la entidad completa.
     */
    public async Task<bool> AprobarAsync(Guid idProducto, CancellationToken cancellationToken)
    {
        var estaPendiente = await context.Productos
            .AsNoTracking()
            .AnyAsync(producto => producto.Id == idProducto && !producto.EstaAprobado, cancellationToken);

        if (!estaPendiente)
        {
            return false;
        }

        var connection = context.Database.GetDbConnection();
        if (connection.State != ConnectionState.Open)
        {
            await connection.OpenAsync(cancellationToken);
        }

        using var command = connection.CreateCommand();
        command.CommandText = "dbo.sp_AprobarProducto";
        command.CommandType = CommandType.StoredProcedure;
        command.Parameters.Add(new SqlParameter("@idProducto", SqlDbType.UniqueIdentifier)
        {
            Value = idProducto
        });

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        return await reader.ReadAsync(cancellationToken);
    }

    public Task<bool> AprobarProductoAsync(Guid idProducto, CancellationToken cancellationToken)
    {
        return AprobarAsync(idProducto, cancellationToken);
    }

    public Task<bool> AprobarProductoAsync(Guid idProducto, CancellationToken cancellationToken)
    {
        return AprobarAsync(idProducto, cancellationToken);
    }

    public Task<bool> AprobarProductoAsync(Guid idProducto, CancellationToken cancellationToken)
    {
        return AprobarAsync(idProducto, cancellationToken);
    }

    /*
     * Descripción:
     * Elimina un producto por identificador.
     * Entradas:
     * Recibe identificador y token de cancelación.
     * Salidas:
     * Devuelve verdadero cuando eliminó un registro.
     * Restricciones:
     * Devuelve falso si el producto no existe.
     */
    public async Task<bool> EliminarAsync(Guid idProducto, CancellationToken cancellationToken)
    {
        var producto = await context.Productos.SingleOrDefaultAsync(producto => producto.Id == idProducto, cancellationToken);
        if (producto is null)
        {
            return false;
        }

        context.Productos.Remove(producto);
        return await context.SaveChangesAsync(cancellationToken) == 1;
    }
}
