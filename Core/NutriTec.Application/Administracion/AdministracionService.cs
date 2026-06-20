using NutriTec.Application.Abstractions.Persistence;
using NutriTec.Application.Abstractions.Services;
using NutriTec.Application.Productos;
using NutriTec.Contracts.Productos;

namespace NutriTec.Application.Administracion;

/*
 * Descripción:
 * Implementa los casos de uso administrativos iniciales para revisar productos alimenticios.
 * Que significa? Este servicio se encarga de gestionar la aprobación de productos que los usuarios han subido, asegurando que cumplen con los estándares antes de ser visibles en la plataforma.
 *
 * Entradas:
 * Recibe un repositorio de productos desacoplado de la persistencia concreta.
 *
 * Salidas:
 * Devuelve productos pendientes y confirma aprobaciones.
 *
 * Restricciones:
 * No permite desaprobar productos ni mezcla operaciones administrativas con controllers de CRUD.
 */

public sealed class AdministracionService(IAdministracionRepository repository) : IAdministracionService
{

    /*
     * Descripción: Consulta productos pendientes de aprobación.
     * Entradas: Recibe token de cancelación.
     * Salidas: Devuelve DTOs de productos pendientes.
     * Restricciones: No modifica datos.
     */

    public async Task<IReadOnlyCollection<ProductoResponse>> ListarProductosPendientesAsync(
        CancellationToken cancellationToken)
    {
        var productos = await repository.ListarProductosPendientesAsync(cancellationToken);
        return productos.Select(ProductoMapper.Mapear).ToArray();
    }


    /*
     * Descripción: Solicita la aprobación de un producto pendiente.
     * Entradas: Recibe identificador y token de cancelación.
     * Salidas: Devuelve verdadero cuando el producto cambia a aprobado.
     * Restricciones: El identificador no puede ser vacío.
     */


    public Task<bool> AprobarProductoAsync(Guid idProducto, CancellationToken cancellationToken)
    {
        if (idProducto == Guid.Empty)
        {
            throw new ArgumentException("El identificador no puede estar vacío.", nameof(idProducto));
        }

        return repository.AprobarProductoAsync(idProducto, cancellationToken);
    }
}
