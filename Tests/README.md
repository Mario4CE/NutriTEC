# Tests

Proyectos de pruebas automatizadas de NutriTEC.

## Proyectos actuales

- `Tests/NutriTec.Application.Tests`: pruebas unitarias de casos de uso y servicios de Application.
- `Tests/NutriTec.Infrastructure.Sql.Tests`: pruebas unitarias de servicios concretos de Infrastructure.Sql, como hashing, JWT y bootstrap de administrador.

## Ejecución

```bash
dotnet test Tests/NutriTec.Application.Tests
dotnet test Tests/NutriTec.Infrastructure.Sql.Tests
dotnet test NutriTEC.sln
```

## Reglas

- Las pruebas unitarias de Application deben usar dobles de prueba o mocks para evitar acceso directo a base de datos.
- Las pruebas de Infrastructure pueden usar proveedores de prueba o entornos controlados, pero no deben crear ni modificar scripts en `Database/` sin confirmación explícita.
- Las pruebas de API e integración se agregarán en incrementos posteriores cuando el alcance lo requiera.
