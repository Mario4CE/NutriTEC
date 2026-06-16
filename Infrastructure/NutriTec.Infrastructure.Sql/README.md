# NutriTec.Infrastructure.Sql

Proyecto de infraestructura para persistencia relacional con SQL Server.

Responsabilidades principales:

- Configurar Entity Framework Core.
- Implementar repositorios relacionales.
- Registrar `DbContext` y repositorios SQL mediante Dependency Injection.
- Traducir errores esperados de SQL Server a excepciones de Application sin exponer detalles internos.

## Manejo de conflictos UNIQUE

Los repositorios SQL deben validar duplicados antes de persistir cuando sea posible y, además, traducir violaciones UNIQUE reales de SQL Server en condiciones de carrera a `ConflictoException`.

Para autenticación, un duplicado detectado por SQL Server durante el registro se expone hacia Application/API como:

```json
{
  "codigo": "conflicto",
  "mensaje": "El correo ya está registrado."
}
```

No se deben devolver nombres de constraints, tablas, mensajes internos de SQL Server ni stack traces.

Restricciones:

- No debe modificar scripts bajo `Database/`.
- No debe ser usado directamente por controllers.
- No debe depender de infraestructura Mongo.
