using NutriTec.Application.Abstractions.Persistence;
using NutriTec.Application.Abstractions.Services;
using NutriTec.Application.Productos;
using NutriTec.Contracts.Administracion;
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

public sealed class AdministracionService(IProductoRepository repository, IAdministracionRepository administracionRepository) : IAdministracionService
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
        var productos = await repository.ListarPendientesAsync(cancellationToken);
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

        return repository.AprobarAsync(idProducto, cancellationToken);
    }

    public Task<IReadOnlyCollection<ReporteCobroNutricionistaResponse>> GenerarReporteCobroAsync(
        decimal montoBasePorPaciente,
        bool incluirSinPacientes,
        CancellationToken cancellationToken)
    {
        if (montoBasePorPaciente <= 0)
        {
            throw new ArgumentException("El monto base por paciente debe ser mayor que cero.", nameof(montoBasePorPaciente));
        }

        return administracionRepository.GenerarReporteCobroAsync(montoBasePorPaciente, incluirSinPacientes, cancellationToken);
    }

    public Task<decimal?> CalcularImcAsync(decimal pesoKg, decimal estaturaCm, CancellationToken cancellationToken)
    {
        if (pesoKg <= 0)
        {
            throw new ArgumentException("El peso debe ser mayor que cero.", nameof(pesoKg));
        }

        if (estaturaCm <= 0)
        {
            throw new ArgumentException("La estatura debe ser mayor que cero.", nameof(estaturaCm));
        }

        return administracionRepository.CalcularImcAsync(pesoKg, estaturaCm, cancellationToken);
    }
}
