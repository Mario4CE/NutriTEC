# Administracion

Esta carpeta contiene casos de uso relacionados con tareas administrativas.

Responsabilidades principales:

- Coordinar operaciones de revisión y aprobación inicial.
- Usar repositorios abstractos para consultar y modificar datos.
- Mantener la lógica administrativa fuera de controllers.

Restricciones:

- No debe acceder directamente a infraestructura.
- No debe mezclar responsabilidades administrativas con CRUD general si existe un servicio dedicado.
- No debe crear estructuras de base de datos.
