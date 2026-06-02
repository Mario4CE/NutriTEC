using NutriTec.Domain.Retroalimentaciones;

namespace NutriTec.Application.Abstractions.Persistence;

/*
 * Descripción:
 * Define las operaciones de persistencia requeridas por el módulo de retroalimentaciones.
 *
 * Entradas:
 * Recibe agregados e identificadores del dominio.
 *
 * Salidas:
 * Devuelve retroalimentaciones almacenadas o confirma actualizaciones.
 *
 * Restricciones:
 * La interfaz no conoce detalles de MongoDB para conservar el desacoplamiento de Application.
 */
public interface IRetroalimentacionRepository
{
    /*
     * Descripción:
     * Persiste un nuevo foro con su mensaje inicial.
     *
     * Entradas:
     * Recibe el agregado y token de cancelación.
     *
     * Salidas:
     * Devuelve el agregado almacenado.
     *
     * Restricciones:
     * El agregado debe llegar validado desde el servicio.
     */
    Task<Retroalimentacion> CrearAsync(Retroalimentacion retroalimentacion, CancellationToken cancellationToken);

    /*
     * Descripción:
     * Consulta foros asociados con un paciente.
     *
     * Entradas:
     * Recibe identificador del paciente y token de cancelación.
     *
     * Salidas:
     * Devuelve una colección de agregados.
     *
     * Restricciones:
     * El identificador debe llegar validado desde el servicio.
     */
    Task<IReadOnlyCollection<Retroalimentacion>> ObtenerPorPacienteAsync(Guid idPaciente, CancellationToken cancellationToken);

    /*
     * Descripción:
     * Consulta foros asociados con un nutricionista.
     *
     * Entradas:
     * Recibe identificador del nutricionista y token de cancelación.
     *
     * Salidas:
     * Devuelve una colección de agregados.
     *
     * Restricciones:
     * El identificador debe llegar validado desde el servicio.
     */
    Task<IReadOnlyCollection<Retroalimentacion>> ObtenerPorNutricionistaAsync(Guid idNutricionista, CancellationToken cancellationToken);

    /*
     * Descripción:
     * Agrega un mensaje al foro indicado.
     *
     * Entradas:
     * Recibe identificador del foro, mensaje y token de cancelación.
     *
     * Salidas:
     * Devuelve verdadero si el documento fue actualizado.
     *
     * Restricciones:
     * La implementación debe evitar reemplazos innecesarios del documento completo.
     */
    Task<bool> AgregarMensajeAsync(Guid idRetroalimentacion, MensajeRetroalimentacion mensaje, CancellationToken cancellationToken);
}
