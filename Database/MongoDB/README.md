# Scripts MongoDB

## Colecciones

- `Collections/001_Retroalimentaciones.mongodb.js`: crea la colección `Retroalimentaciones` usada por `NutriTec.MongoApi` y define índices para consultas por paciente y nutricionista.

## Nota de diseño

El script usa una sola colección con mensajes embebidos porque el código actual modela la retroalimentación como un agregado documental: una retroalimentación contiene su lista de mensajes. Esto evita crear una segunda colección `respuesta_foro` que después obligaría a cambiar repositorio, servicios y contratos de la API Mongo.

Los campos se mantienen en PascalCase para alinearse con las propiedades actuales serializadas por el driver oficial de MongoDB para .NET.
