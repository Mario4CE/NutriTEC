using MongoDB.Driver;
using NutriTec.Application.Abstractions.Persistence;
using NutriTec.Domain.Retroalimentaciones;

namespace NutriTec.Infrastructure.Mongo.Repositories;

/*
 * Descripción:
 * Implementa la persistencia documental de los foros de retroalimentación mediante MongoDB Driver.
 *
 * Entradas:
 * Recibe la base de datos MongoDB resuelta por Dependency Injection y agregados del dominio.
 *
 * Salidas:
 * Inserta, consulta y actualiza documentos de la colección Retroalimentaciones.
 *
 * Restricciones:
 * Es una implementación exclusiva de Infrastructure.Mongo; Application depende únicamente de la interfaz.
 */
public sealed class RetroalimentacionMongoRepository(IMongoDatabase database) : IRetroalimentacionRepository
{
    private const string CollectionName = "Retroalimentaciones";
    private readonly IMongoCollection<Retroalimentacion> _collection = database.GetCollection<Retroalimentacion>(CollectionName);

    /*
     * Descripción:
     * Inserta un foro completo con su mensaje inicial.
     *
     * Entradas:
     * Recibe el agregado y token de cancelación.
     *
     * Salidas:
     * Devuelve el mismo agregado una vez insertado.
     *
     * Restricciones:
     * El servicio de aplicación debe validar el agregado antes de invocar este método.
     */
    public async Task<Retroalimentacion> CrearAsync(
        Retroalimentacion retroalimentacion,
        CancellationToken cancellationToken)
    {
        await _collection.InsertOneAsync(retroalimentacion, cancellationToken: cancellationToken);
        return retroalimentacion;
    }

    /*
     * Descripción:
     * Obtiene foros asociados con un paciente, comenzando por los más recientes.
     *
     * Entradas:
     * Recibe el identificador del paciente y token de cancelación.
     *
     * Salidas:
     * Devuelve una colección de agregados.
     *
     * Restricciones:
     * No aplica reglas de negocio ni transforma DTOs.
     */
    public async Task<IReadOnlyCollection<Retroalimentacion>> ObtenerPorPacienteAsync(
        Guid idPaciente,
        CancellationToken cancellationToken)
    {
        return await _collection
            .Find(retroalimentacion => retroalimentacion.IdPaciente == idPaciente)
            .SortByDescending(retroalimentacion => retroalimentacion.FechaCreacionUtc)
            .ToListAsync(cancellationToken);
    }

    /*
     * Descripción:
     * Obtiene foros asociados con un nutricionista, comenzando por los más recientes.
     *
     * Entradas:
     * Recibe el identificador del nutricionista y token de cancelación.
     *
     * Salidas:
     * Devuelve una colección de agregados.
     *
     * Restricciones:
     * No aplica reglas de negocio ni transforma DTOs.
     */
    public async Task<IReadOnlyCollection<Retroalimentacion>> ObtenerPorNutricionistaAsync(
        Guid idNutricionista,
        CancellationToken cancellationToken)
    {
        return await _collection
            .Find(retroalimentacion => retroalimentacion.IdNutricionista == idNutricionista)
            .SortByDescending(retroalimentacion => retroalimentacion.FechaCreacionUtc)
            .ToListAsync(cancellationToken);
    }

    /*
     * Descripción:
     * Agrega atómicamente un mensaje al arreglo de respuestas de un foro.
     *
     * Entradas:
     * Recibe identificador del foro, mensaje y token de cancelación.
     *
     * Salidas:
     * Devuelve verdadero cuando existe un documento actualizado.
     *
     * Restricciones:
     * Usa Push para evitar reemplazar el documento completo y reducir conflictos concurrentes.
     */
    public async Task<bool> AgregarMensajeAsync(
        Guid idRetroalimentacion,
        MensajeRetroalimentacion mensaje,
        CancellationToken cancellationToken)
    {
        var update = Builders<Retroalimentacion>.Update.Push(retroalimentacion => retroalimentacion.Mensajes, mensaje);
        var result = await _collection.UpdateOneAsync(
            retroalimentacion => retroalimentacion.Id == idRetroalimentacion,
            update,
            cancellationToken: cancellationToken);

        return result.ModifiedCount == 1;
    }
}
