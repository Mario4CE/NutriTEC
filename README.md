# NutriTEC

NutriTEC es una plataforma para acompañar hábitos de nutrición saludable. Sus usuarios pueden gestionar su alimentación de forma independiente o trabajar con un nutricionista.

## Estado actual

El repositorio contiene una arquitectura inicial de APIs separadas por motor de persistencia, con lógica compartida en capas reutilizables:

- **API SQL Server:** primer corte vertical del módulo de productos.
- **API MongoDB:** primer corte vertical del módulo de retroalimentaciones tipo foro.
- **Application:** servicios e interfaces compartidos, sin dependencia de bases de datos concretas.
- **Domain:** entidades centrales del dominio.
- **Contracts:** DTOs y respuestas HTTP estandarizadas.
- **Infrastructure:** adaptadores independientes para SQL Server y MongoDB.

Todavía no están implementados Auth, clientes, nutricionistas, recetas, planes alimenticios, consumo diario, administración ni reportes.

## Arquitectura

```text
Controller → Service → Repository → Database
```

```text
NutriTEC/
├── Api/
│   ├── NutriTec.SqlApi
│   └── NutriTec.MongoApi
├── Core/
│   ├── NutriTec.Application
│   ├── NutriTec.Domain
│   └── NutriTec.Contracts
├── Infrastructure/
│   ├── NutriTec.Infrastructure.Sql
│   └── NutriTec.Infrastructure.Mongo
├── Database/
│   ├── SqlServer/
│   └── MongoDB/
├── Docs/
├── Mobile/
├── Web/
└── NutriTec.slnx
```

Cada API registra únicamente su adaptador de persistencia:

- `NutriTec.SqlApi` registra `AddNutriTecSqlInfrastructure()`.
- `NutriTec.MongoApi` registra `AddNutriTecMongoInfrastructure()`.
- Ambas APIs reutilizan `AddNutriTecApplication()`.

## Requisitos locales

- .NET SDK 10.
- SQL Server accesible para ejecutar la API SQL.
- MongoDB accesible para ejecutar la API Mongo.

## Configuración de desarrollo

Los archivos `appsettings.Development.json` contienen valores locales de ejemplo. No deben utilizarse credenciales reales dentro del repositorio.

Antes de iniciar la API SQL, configure `ConnectionStrings:NutriTecSqlServer`. Antes de iniciar la API Mongo, configure `MongoDb:ConnectionString` y `MongoDb:DatabaseName`. Para secretos locales puede utilizar variables de entorno o .NET User Secrets.

## Inicialización SQL Server

Ejecute los scripts versionados en orden ascendente. El primer script disponible crea la tabla de productos:

```text
Database/SqlServer/Tables/001_Productos.sql
```

Los stored procedures, vistas, funciones y triggers requeridos se agregarán gradualmente con los módulos que los necesiten.

## Ejecutar las APIs

```bash
dotnet restore NutriTec.slnx
dotnet run --project Api/NutriTec.SqlApi/NutriTec.SqlApi.csproj
dotnet run --project Api/NutriTec.MongoApi/NutriTec.MongoApi.csproj
```

## Endpoints implementados

### Productos — SQL Server

| Método | Ruta | Descripción |
| --- | --- | --- |
| `POST` | `/api/productos` | Registra un producto pendiente de aprobación. |
| `GET` | `/api/productos` | Lista productos. |
| `GET` | `/api/productos/{idProducto}` | Consulta un producto por identificador. |
| `GET` | `/api/productos/buscar?nombre={nombre}` | Busca productos por nombre. |
| `GET` | `/api/productos/codigo-barras/{codigoBarras}` | Consulta un producto por código de barras. |
| `PUT` | `/api/productos/{idProducto}` | Edita un producto. |
| `DELETE` | `/api/productos/{idProducto}` | Elimina un producto. |

### Retroalimentaciones — MongoDB

| Método | Ruta | Descripción |
| --- | --- | --- |
| `POST` | `/api/retroalimentaciones` | Abre un foro con su primer mensaje. |
| `GET` | `/api/retroalimentaciones/pacientes/{idPaciente}` | Consulta foros de un paciente. |
| `GET` | `/api/retroalimentaciones/nutricionistas/{idNutricionista}` | Consulta foros de un nutricionista. |
| `POST` | `/api/retroalimentaciones/{idRetroalimentacion}/mensajes` | Agrega una respuesta a un foro. |

Los archivos `.http` dentro de cada API incluyen solicitudes de ejemplo para realizar pruebas manuales.

## Convenciones internas

- Los controllers no acceden directamente a las bases de datos.
- Las entidades del dominio no se exponen directamente al frontend.
- La lógica común vive en `Core` y los detalles de persistencia viven en `Infrastructure`.
- La documentación interna relevante utiliza bloques `/* ... */` con las secciones `Descripción`, `Entradas`, `Salidas` y `Restricciones`.
- No se versionan contraseñas ni cadenas de conexión reales.

## Próximos pasos sugeridos

1. Agregar el caso de uso administrativo para aprobar productos.
2. Incorporar triggers, vistas y stored procedures SQL Server conforme se implementen los módulos relacionales.
3. Implementar Auth con contraseñas hasheadas y JWT.
4. Agregar pruebas automatizadas cuando el entorno de desarrollo incluya el SDK de .NET.
