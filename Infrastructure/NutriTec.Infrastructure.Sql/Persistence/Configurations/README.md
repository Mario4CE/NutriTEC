# Configurations

Esta carpeta contiene configuraciones de Entity Framework Core para entidades relacionales.

Responsabilidades principales:

- Mapear entidades de dominio a tablas existentes o esperadas.
- Definir claves, longitudes, índices y restricciones del modelo EF.
- Mantener la configuración relacional separada del dominio.

Restricciones:

- No debe crear tablas mediante scripts SQL.
- No debe contener controllers ni servicios de aplicación.
- No debe configurar colecciones MongoDB.
