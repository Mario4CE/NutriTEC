# NutriTec.Infrastructure.Sql

Proyecto de infraestructura para persistencia relacional con SQL Server.

Responsabilidades principales:

- Configurar Entity Framework Core.
- Implementar repositorios relacionales.
- Registrar `DbContext` y repositorios SQL mediante Dependency Injection.

Restricciones:

- No debe modificar scripts bajo `Database/`.
- No debe ser usado directamente por controllers.
- No debe depender de infraestructura Mongo.
