# Productos

Esta carpeta contiene DTOs públicos del módulo de productos.

Responsabilidades principales:

- Definir solicitudes para crear y actualizar productos.
- Definir respuestas serializables para productos alimenticios.
- Separar los contratos HTTP de la entidad de dominio `Producto`.

Restricciones:

- No debe incluir Entity Framework Core.
- No debe exponer detalles de persistencia SQL.
- No debe permitir modificar responsabilidades administrativas no contempladas por el DTO.
