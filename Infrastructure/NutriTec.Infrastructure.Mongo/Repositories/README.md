# Repositories

Esta carpeta contiene implementaciones Mongo de repositorios definidos por Application.

Responsabilidades principales:

- Ejecutar operaciones documentales sobre colecciones MongoDB.
- Traducir abstracciones de Application a filtros y actualizaciones Mongo.
- Mantener el acceso documental encapsulado en infraestructura.

Restricciones:

- No debe ser invocada directamente por controllers.
- No debe acceder a SQL Server.
- No debe exponer documentos internos si Application requiere DTOs.
