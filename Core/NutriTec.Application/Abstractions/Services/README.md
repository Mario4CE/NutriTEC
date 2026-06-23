# Services

Esta carpeta contiene interfaces de servicios de aplicación y servicios técnicos abstractos.

Responsabilidades principales:

- Definir casos de uso consumidos por controllers.
- Separar contratos de Application de sus implementaciones.
- Expresar capacidades técnicas como hashing sin elegir una implementación concreta.

Restricciones:

- No debe implementar lógica concreta.
- No debe acceder a bases de datos.
- No debe depender de controllers o middleware.
