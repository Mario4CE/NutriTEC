# Abstractions

Esta carpeta agrupa contratos internos que separan Application de implementaciones concretas.

Responsabilidades principales:

- Definir puertos de entrada para servicios de aplicación.
- Definir puertos de salida para persistencia y servicios técnicos.
- Permitir inyección de dependencias sin acoplarse a infraestructura.

Restricciones:

- No debe contener implementaciones concretas.
- No debe depender de proyectos de infraestructura.
- No debe mezclar abstracciones SQL con abstracciones Mongo si no corresponde.
