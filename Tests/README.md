# Tests

Proyectos de pruebas automatizadas de NutriTEC.

## Proyectos actuales

- `Tests/NutriTec.Application.Tests`: pruebas unitarias de casos de uso y servicios de Application, incluyendo autenticación, productos, administración y retroalimentaciones.
- `Tests/NutriTec.Infrastructure.Sql.Tests`: pruebas unitarias de servicios concretos de Infrastructure.Sql, como hashing, JWT y bootstrap de administrador.


## Documentación de casos de prueba

La explicación de cada prueba está en [`Tests/TEST_CASES.md`](TEST_CASES.md). Para pruebas manuales desde navegador con Swagger UI, consulte [`Tests/SwaggerExamples.md`](SwaggerExamples.md).

## Ejecución

En Linux/macOS o Git Bash:

```bash
./Tests/run-tests.sh
```

El script restaura paquetes, compila la solución en `Release`, descubre todos los proyectos `*.csproj` dentro de `Tests/`, compila y ejecuta cada proyecto de pruebas, y finalmente ejecuta las pruebas a nivel de solución.

Comandos manuales equivalentes:

```bash
dotnet test Tests/NutriTec.Application.Tests
dotnet test Tests/NutriTec.Infrastructure.Sql.Tests
dotnet test NutriTEC.sln
```


## Smoke check de bases de datos

Para verificar rápido que SQL Server y MongoDB tienen la estructura esperada, ejecute:

```powershell
pwsh Tests/check-databases.ps1
```

El script valida conexión SQL, tablas base, funciones, vistas, stored procedures, triggers, conexión Mongo, colección `Retroalimentaciones` e índices principales. Requiere `sqlcmd` y `mongosh` disponibles en `PATH`.

Si usa otra instancia o nombres distintos:

```powershell
pwsh Tests/check-databases.ps1 -SqlServer ".\SQLEXPRESS" -SqlDatabase "NutriTec" -MongoConnectionString "mongodb://localhost:27017" -MongoDatabase "nutritec_feedback"
```

## Reglas

- Las pruebas unitarias de Application deben usar dobles de prueba o mocks para evitar acceso directo a base de datos.
- Las pruebas de Infrastructure pueden usar proveedores de prueba o entornos controlados, pero no deben crear ni modificar scripts en `Database/` sin confirmación explícita.
- Las pruebas de API e integración se agregarán en incrementos posteriores cuando el alcance lo requiera.
