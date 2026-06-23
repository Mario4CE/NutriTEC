# Controllers SQL API

Controllers HTTP de `NutriTec.SqlApi`.

## Controllers actuales

| Controller | Estado | Responsabilidad |
| --- | --- | --- |
| `AuthController` | Implementado | Login, registro de cliente/nutricionista y `/me`. |
| `ProductosController` | Implementado | CRUD de productos; listados públicos filtran aprobados. |
| `AdministracionController` | Implementado | Productos pendientes y aprobación administrativa. |
| `ObjetosSqlController` | Implementado | Endpoints que ejecutan objetos programables SQL. |
| `VistasController` | Implementado como cierre de brechas | Endpoints faltantes de vistas Cliente/Nutricionista. |

## Rutas agregadas para vistas

- Cliente: planes asignados, registros diarios, recetas y medidas.
- Nutricionista: pacientes, asociación, planes, tiempos de comida y seguimiento de registro diario.

## Restricciones

- No devolver entidades internas ni datos sensibles.
- Proteger endpoints con JWT y políticas de rol cuando corresponda.
- Mantener respuestas consistentes con `ApiResponse<T>` o contratos públicos.
- Para nuevos endpoints, preferir mover la lógica a `Application`/`Infrastructure` en lugar de agregar SQL directo en controllers.
