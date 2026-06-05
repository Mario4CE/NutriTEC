# NutriTec.MongoApi

Proyecto ASP.NET Core encargado de exponer los módulos respaldados por MongoDB.

Responsabilidades principales:

- Configurar controllers para recursos documentales.
- Registrar la capa `Application` y la infraestructura Mongo mediante Dependency Injection.
- Publicar endpoints de retroalimentaciones.
- Aplicar middleware de validación y errores esperados.

Restricciones:

- No debe depender de la infraestructura SQL.
- No debe acceder a colecciones Mongo desde controllers.
- No debe mezclar modelos documentales con modelos relacionales.
