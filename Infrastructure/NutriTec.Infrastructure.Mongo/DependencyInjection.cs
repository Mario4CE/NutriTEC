using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using NutriTec.Application.Abstractions.Persistence;
using NutriTec.Infrastructure.Mongo.Configuration;
using NutriTec.Infrastructure.Mongo.Repositories;

namespace NutriTec.Infrastructure.Mongo;

/*
 * Descripción:
 * Expone el registro de dependencias exclusivas de la persistencia MongoDB.
 *
 * Entradas:
 * Recibe la colección de servicios y la configuración del host de la API Mongo.
 *
 * Salidas:
 * Retorna la colección con el cliente y la base de datos MongoDB registrados.
 *
 * Restricciones:
 * Este punto de composición solo debe invocarse desde NutriTec.MongoApi.
 */

public static class DependencyInjection
{
    /*
     * Descripción:
     * Enlaza y valida las opciones de MongoDB, registra el cliente oficial y expone la base de datos configurada.
     * También configura el serializador de Guid para compatibilidad con el driver de MongoDB.
     *
     * Entradas:
     * Recibe la colección de servicios y una configuración que debe incluir la sección MongoDb.
     *
     * Salidas:
     * Retorna la colección para permitir una configuración encadenada.
     *
     * Restricciones:
     * La configuración se valida al iniciar la aplicación y los secretos de ambientes reales deben suministrarse externamente.
     */
    public static IServiceCollection AddNutriTecMongoInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Configurar serialización de Guid para MongoDB
        // Sin esto, el driver no puede serializar Guid en queries LINQ
        BsonSerializer.TryRegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));

        services
            .AddOptions<MongoDbOptions>()
            .Bind(configuration.GetSection(MongoDbOptions.SectionName))
            .Validate(
                options => !string.IsNullOrWhiteSpace(options.ConnectionString),
                "La configuración 'MongoDb:ConnectionString' es obligatoria.")
            .Validate(
                options => !string.IsNullOrWhiteSpace(options.DatabaseName),
                "La configuración 'MongoDb:DatabaseName' es obligatoria.")
            .ValidateOnStart();

        services.AddSingleton<IMongoClient>(serviceProvider =>
        {
            var options = serviceProvider.GetRequiredService<IOptions<MongoDbOptions>>().Value;
            return new MongoClient(options.ConnectionString);
        });

        services.AddSingleton<IMongoDatabase>(serviceProvider =>
        {
            var options = serviceProvider.GetRequiredService<IOptions<MongoDbOptions>>().Value;
            var client = serviceProvider.GetRequiredService<IMongoClient>();
            return client.GetDatabase(options.DatabaseName);
        });

        services.AddScoped<IRetroalimentacionRepository, RetroalimentacionMongoRepository>();

        return services;
    }
}