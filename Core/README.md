# Core

Esta carpeta contiene el núcleo de NutriTEC y no debe depender de detalles concretos de infraestructura.

Responsabilidades principales:

- Definir contratos compartidos entre APIs y casos de uso.
- Implementar reglas de aplicación y servicios de negocio.
- Mantener entidades de dominio independientes de frameworks externos.

Restricciones:

- No debe depender de proyectos de `Api`.
- No debe depender de implementaciones concretas de SQL Server o MongoDB.
- No debe contener secretos ni configuración de despliegue.
