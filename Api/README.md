# Api

Esta carpeta contiene las APIs ASP.NET Core de NutriTEC.

## Proyectos

| Proyecto | Estado | Responsabilidad |
| --- | --- | --- |
| `NutriTec.SqlApi` | Implementado parcialmente y activo | Auth/JWT, productos, administración, vistas Cliente/Nutricionista y objetos SQL programables. |
| `NutriTec.MongoApi` | Implementado parcialmente y activo | Retroalimentaciones/foro con persistencia documental MongoDB. |

## Responsabilidades principales

- Recibir solicitudes HTTP mediante controllers.
- Validar contratos de entrada con ASP.NET Core.
- Delegar casos de uso a `Core/NutriTec.Application` siempre que exista servicio de aplicación para el flujo.
- Envolver respuestas usando contratos compartidos.
- Mantener separadas las APIs que trabajan con SQL Server y MongoDB.

## Restricciones

- No guardar secretos reales en configuración versionada.
- No exponer entidades internas ni contraseñas/password hashes.
- Mantener JWT y políticas de rol en endpoints protegidos.
- Los nuevos módulos deben priorizar el flujo `Controller → Service → Repository → Database`.

## Estado de cobertura

- Cliente: cubierto por API SQL; frontend cliente todavía usa almacenamiento local.
- Nutricionista: cubierto por API SQL; no hay frontend dedicado.
- Admin: cubierto por API SQL para aprobación de productos; no hay frontend dedicado.
- Retroalimentaciones: cubierto por API MongoDB.
