# Controllers

Esta carpeta contiene los controllers HTTP de la API SQL.

Responsabilidades principales:

- Definir rutas REST para módulos relacionales.
- Recibir DTOs de entrada desde `NutriTec.Contracts`.
- Invocar servicios de `NutriTec.Application`.
- Devolver `ApiResponse<T>` con códigos HTTP adecuados.

Restricciones:

- No deben consultar repositorios ni bases de datos directamente.
- No deben devolver entidades de dominio.
- No deben contener reglas de negocio extensas.
