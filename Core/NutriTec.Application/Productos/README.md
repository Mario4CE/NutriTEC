# Productos

Esta carpeta contiene la lógica de aplicación del módulo de productos.

Responsabilidades principales:

- Implementar casos de uso de creación, consulta, búsqueda, edición y eliminación.
- Convertir entidades de dominio en DTOs mediante mappers internos.
- Aplicar reglas como unicidad de código de barras.

Restricciones:

- No debe acceder a Entity Framework Core directamente.
- No debe exponer entidades de dominio al frontend.
- No debe implementar rutas HTTP.
