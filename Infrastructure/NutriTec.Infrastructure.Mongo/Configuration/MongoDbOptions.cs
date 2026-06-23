namespace NutriTec.Infrastructure.Mongo.Configuration;

/*
 * Descripción:
 * Modela la configuración necesaria para establecer la conexión con MongoDB.
 *
 * Entradas:
 * Recibe valores desde la sección MongoDb de la configuración del host.
 *
 * Salidas:
 * Proporciona la cadena de conexión y el nombre de base de datos validados al cliente MongoDB.
 *
 * Restricciones:
 * Los valores reales de ambientes desplegados deben inyectarse mediante configuración externa y no almacenarse en el repositorio.
 */

public sealed class MongoDbOptions
{


    /*
     * Descripción:
     * Define el nombre de la sección que agrupa las opciones de MongoDB.
     *
     * Entradas:
     * No recibe entradas.
     *
     * Salidas:
     * Permite reutilizar una única clave al enlazar la configuración.
     *
     * Restricciones:
     * Debe coincidir con la sección definida por la API Mongo.
     */
    public const string SectionName = "MongoDb";


    /*
     * Descripción:
     * Obtiene o establece la URI utilizada para conectar con MongoDB.
     *
     * Entradas:
     * Recibe un valor desde configuración.
     *
     * Salidas:
     * Proporciona la URI al cliente oficial de MongoDB.
     *
     * Restricciones:
     * No puede ser nula, vacía ni contener secretos versionados para ambientes reales.
     */

    public string ConnectionString { get; init; } = string.Empty;


    /*
     * Descripción:
     * Obtiene o establece el nombre lógico de la base de datos de NutriTEC.
     *
     * Entradas:
     * Recibe un valor desde configuración.
     *
     * Salidas:
     * Permite resolver la instancia IMongoDatabase compartida.
     *
     * Restricciones:
     * No puede ser nulo ni vacío.
     */

    public string DatabaseName { get; init; } = string.Empty;
}
