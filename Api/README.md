# Api

Esta carpeta contiene los proyectos ASP.NET Core que exponen los endpoints HTTP de NutriTEC.

Responsabilidades principales:

- Recibir solicitudes del cliente mediante controllers.
- Validar contratos de entrada con el pipeline de ASP.NET Core.
- Delegar los casos de uso a la capa `Application`.
- Envolver respuestas usando los contratos compartidos.
- Mantener separadas las APIs que trabajan con SQL Server y MongoDB.

Restricciones:

- Los controllers no deben acceder directamente a bases de datos.
- No se deben exponer entidades del dominio como respuesta HTTP.
- No se deben guardar secretos reales en configuración versionada.
