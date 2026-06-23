# Controllers

Esta carpeta contiene los controllers HTTP de la API Mongo.

Responsabilidades principales:

- Definir rutas REST para módulos documentales.
- Recibir DTOs de entrada desde `NutriTec.Contracts`.
- Invocar servicios de `NutriTec.Application`.
- Devolver respuestas normalizadas hacia el cliente.

Restricciones:

- No deben acceder directamente a MongoDB.
- No deben exponer documentos internos si no son DTOs.
- No deben implementar reglas de negocio fuera de Application.
