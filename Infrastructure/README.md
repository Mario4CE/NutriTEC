# Infrastructure

Esta carpeta contiene implementaciones concretas de acceso a datos y configuración técnica.

Responsabilidades principales:

- Implementar repositorios definidos por Application.
- Configurar proveedores concretos como SQL Server y MongoDB.
- Registrar dependencias de infraestructura mediante Dependency Injection.

Restricciones:

- No debe contener controllers.
- No debe definir contratos públicos para el frontend.
- No debe mezclar infraestructura SQL y Mongo en una misma implementación.
