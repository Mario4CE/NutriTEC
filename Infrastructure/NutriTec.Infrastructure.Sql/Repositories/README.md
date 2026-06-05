# Repositories

Esta carpeta contiene implementaciones SQL de repositorios definidos en Application.

Responsabilidades principales:

- Ejecutar operaciones relacionales usando `NutriTecDbContext`.
- Traducir abstracciones de Application a consultas EF Core.
- Persistir cambios sin exponer EF Core a capas superiores.

Restricciones:

- No debe ser invocada directamente por controllers.
- No debe contener reglas de negocio que pertenezcan a Application.
- No debe acceder a MongoDB.
