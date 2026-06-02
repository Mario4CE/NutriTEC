using NutriTec.Application.Abstractions.Persistence;
using NutriTec.Application.Abstractions.Services;
using NutriTec.Application.Common;
using NutriTec.Contracts.Productos;
using NutriTec.Domain.Productos;

namespace NutriTec.Application.Productos;

/*
 * Descripción:
 * Implementa las reglas iniciales para registrar, consultar, editar y eliminar productos alimenticios.
 *
 * Entradas:
 * Recibe DTOs de producto y un repositorio relacional desacoplado.
 *
 * Salidas:
 * Devuelve DTOs preparados para la API o confirma modificaciones.
 *
 * Restricciones:
 * Rechaza textos vacíos, nutrientes negativos y códigos de barras duplicados; cada producto nuevo queda pendiente de aprobación.
 */
public sealed class ProductoService(IProductoRepository repository) : IProductoService
{
    /*
     * Descripción:
     * Registra un producto nuevo pendiente de aprobación.
     * Entradas:
     * Recibe DTO de creación y token de cancelación.
     * Salidas:
     * Devuelve el producto creado como DTO.
     * Restricciones:
     * El código de barras debe ser único.
     */
    public async Task<ProductoResponse> CrearAsync(CrearProductoRequest request, CancellationToken cancellationToken)
    {
        Validar(request.Nombre, request.CodigoBarras, request.Calorias, request.Proteinas, request.Carbohidratos, request.Grasas);
        var codigoBarras = request.CodigoBarras.Trim();
        await ValidarCodigoBarrasUnicoAsync(codigoBarras, null, cancellationToken);

        var producto = new Producto
        {
            Id = Guid.NewGuid(),
            Nombre = request.Nombre.Trim(),
            CodigoBarras = codigoBarras,
            Calorias = request.Calorias,
            Proteinas = request.Proteinas,
            Carbohidratos = request.Carbohidratos,
            Grasas = request.Grasas,
            EstaAprobado = false,
            FechaCreacionUtc = DateTime.UtcNow
        };

        return ProductoMapper.Mapear(await repository.CrearAsync(producto, cancellationToken));
    }

    /*
     * Descripción:
     * Consulta un producto por identificador.
     * Entradas:
     * Recibe identificador y token de cancelación.
     * Salidas:
     * Devuelve el DTO encontrado o nulo.
     * Restricciones:
     * El identificador no puede ser vacío.
     */
    public async Task<ProductoResponse?> ObtenerPorIdAsync(Guid idProducto, CancellationToken cancellationToken)
    {
        ValidarIdentificador(idProducto);
        var producto = await repository.ObtenerPorIdAsync(idProducto, cancellationToken);
        return producto is null ? null : ProductoMapper.Mapear(producto);
    }

    /*
     * Descripción:
     * Lista los productos registrados.
     * Entradas:
     * Recibe token de cancelación.
     * Salidas:
     * Devuelve una colección de DTOs.
     * Restricciones:
     * No modifica datos.
     */
    public async Task<IReadOnlyCollection<ProductoResponse>> ListarAsync(CancellationToken cancellationToken)
    {
        var productos = await repository.ListarAsync(cancellationToken);
        return productos.Select(ProductoMapper.Mapear).ToArray();
    }

    /*
     * Descripción:
     * Busca productos cuyo nombre contiene el criterio indicado.
     * Entradas:
     * Recibe criterio y token de cancelación.
     * Salidas:
     * Devuelve DTOs coincidentes.
     * Restricciones:
     * El criterio no puede estar vacío.
     */
    public async Task<IReadOnlyCollection<ProductoResponse>> BuscarPorNombreAsync(string nombre, CancellationToken cancellationToken)
    {
        ValidarTexto(nombre, nameof(nombre));
        var productos = await repository.BuscarPorNombreAsync(nombre.Trim(), cancellationToken);
        return productos.Select(ProductoMapper.Mapear).ToArray();
    }

    /*
     * Descripción:
     * Consulta un producto por código de barras.
     * Entradas:
     * Recibe código y token de cancelación.
     * Salidas:
     * Devuelve el DTO encontrado o nulo.
     * Restricciones:
     * El código no puede estar vacío.
     */
    public async Task<ProductoResponse?> ObtenerPorCodigoBarrasAsync(string codigoBarras, CancellationToken cancellationToken)
    {
        ValidarTexto(codigoBarras, nameof(codigoBarras));
        var producto = await repository.ObtenerPorCodigoBarrasAsync(codigoBarras.Trim(), cancellationToken);
        return producto is null ? null : ProductoMapper.Mapear(producto);
    }

    /*
     * Descripción:
     * Actualiza los datos editables de un producto.
     * Entradas:
     * Recibe identificador, DTO de edición y token de cancelación.
     * Salidas:
     * Devuelve verdadero cuando el producto existe y fue actualizado.
     * Restricciones:
     * No modifica el estado de aprobación y conserva la unicidad del código de barras.
     */
    public async Task<bool> ActualizarAsync(
        Guid idProducto,
        ActualizarProductoRequest request,
        CancellationToken cancellationToken)
    {
        ValidarIdentificador(idProducto);
        Validar(request.Nombre, request.CodigoBarras, request.Calorias, request.Proteinas, request.Carbohidratos, request.Grasas);
        var producto = await repository.ObtenerPorIdAsync(idProducto, cancellationToken);
        if (producto is null)
        {
            return false;
        }

        var codigoBarras = request.CodigoBarras.Trim();
        await ValidarCodigoBarrasUnicoAsync(codigoBarras, idProducto, cancellationToken);
        producto.Nombre = request.Nombre.Trim();
        producto.CodigoBarras = codigoBarras;
        producto.Calorias = request.Calorias;
        producto.Proteinas = request.Proteinas;
        producto.Carbohidratos = request.Carbohidratos;
        producto.Grasas = request.Grasas;

        return await repository.ActualizarAsync(producto, cancellationToken);
    }

    /*
     * Descripción:
     * Elimina un producto por identificador.
     * Entradas:
     * Recibe identificador y token de cancelación.
     * Salidas:
     * Devuelve verdadero cuando el producto fue eliminado.
     * Restricciones:
     * El identificador no puede ser vacío.
     */
    public Task<bool> EliminarAsync(Guid idProducto, CancellationToken cancellationToken)
    {
        ValidarIdentificador(idProducto);
        return repository.EliminarAsync(idProducto, cancellationToken);
    }

    private async Task ValidarCodigoBarrasUnicoAsync(string codigoBarras, Guid? idProductoExcluido, CancellationToken cancellationToken)
    {
        if (await repository.ExisteCodigoBarrasAsync(codigoBarras, idProductoExcluido, cancellationToken))
        {
            throw new ConflictoException("Ya existe un producto con el código de barras indicado.");
        }
    }

    private static void Validar(
        string nombre,
        string codigoBarras,
        decimal calorias,
        decimal proteinas,
        decimal carbohidratos,
        decimal grasas)
    {
        ValidarTexto(nombre, nameof(nombre));
        ValidarTexto(codigoBarras, nameof(codigoBarras));
        if (calorias < 0 || proteinas < 0 || carbohidratos < 0 || grasas < 0)
        {
            throw new ArgumentException("Los valores nutricionales no pueden ser negativos.");
        }
    }

    private static void ValidarIdentificador(Guid identificador)
    {
        if (identificador == Guid.Empty)
        {
            throw new ArgumentException("El identificador no puede estar vacío.", nameof(identificador));
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
