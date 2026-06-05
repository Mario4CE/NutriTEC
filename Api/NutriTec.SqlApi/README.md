# NutriTec.SqlApi

Proyecto ASP.NET Core encargado de exponer los módulos respaldados por SQL Server.

Responsabilidades principales:

- Configurar controllers para recursos relacionales.
- Registrar la capa `Application` y la infraestructura SQL mediante Dependency Injection.
- Publicar endpoints para productos y administración inicial.
- Aplicar middleware de errores de aplicación.

Restricciones:

- No debe depender de la infraestructura Mongo.
- No debe acceder a `DbContext` desde controllers.
- No debe implementar lógica de negocio dentro de endpoints.
