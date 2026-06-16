# NutriTec.Infrastructure.Sql

Proyecto de infraestructura para persistencia relacional con SQL Server.

Responsabilidades principales:

- Configurar Entity Framework Core.
- Implementar repositorios relacionales.
- Registrar `DbContext` y repositorios SQL mediante Dependency Injection.
- Traducir errores esperados de SQL Server a excepciones de Application sin exponer detalles internos.

## Catálogos SQL

`TIPO_COBRO` es un catálogo relacional estable para los valores permitidos de cobro de nutricionistas:

- `semanal`
- `mensual`
- `anual`

Infrastructure.Sql lo mapea como entidad de persistencia y `NUTRICIONISTA.tipo_cobro` lo referencia mediante llave foránea. Estos valores se versionan como seed porque son datos técnicos requeridos para validar registros de nutricionistas y no corresponden a datos personales ni datos creados por clientes.

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

## Estrategia de seed/bootstrap inicial

Por seguridad y mantenibilidad, los datos que normalmente se crean desde pantallas o flujos del sistema no deben versionarse como seeds SQL iniciales. Esto incluye clientes, nutricionistas, productos, planes, mediciones, tarjetas o datos personales.

El único seed recomendado en esta etapa es un **bootstrap controlado del primer administrador**, porque se necesita una cuenta inicial para operar módulos administrativos antes de que exista una pantalla de administración.

Reglas para ese bootstrap:

- Debe estar deshabilitado por defecto.
- Debe ser idempotente: si ya existe un administrador, no debe crear otro.
- El correo y la contraseña temporal deben venir de variables de entorno, user-secrets o gestor de secretos.
- No se debe guardar la contraseña temporal ni un hash real en archivos versionados.
- El hash debe generarse con `IPasswordHasher`/`PasswordHasher`; nunca con contraseña en texto plano.
- No debe crear clientes, nutricionistas, productos ni datos de negocio.
- No debe requerir modificar `Database/` ni crear scripts SQL nuevos sin confirmación explícita.
