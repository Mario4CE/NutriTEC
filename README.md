# NutriTEC

NutriTEC es una plataforma para acompañar hábitos de nutrición saludable. Sus usuarios pueden gestionar su alimentación de forma independiente o trabajar con un nutricionista.

## Estado actual

El repositorio contiene una arquitectura inicial de APIs separadas por motor de persistencia, con lógica compartida en capas reutilizables:

- **API SQL Server:** CRUD inicial de productos y revisión administrativa de productos pendientes.
- **API MongoDB:** primer corte vertical del módulo de retroalimentaciones tipo foro.
- **Application:** servicios e interfaces compartidos, sin dependencia de bases de datos concretas.
- **Domain:** entidades centrales del dominio.
- **Contracts:** DTOs y respuestas HTTP estandarizadas.
- **Infrastructure:** adaptadores independientes para SQL Server y MongoDB.

Todavía no están implementados Auth, clientes, nutricionistas, recetas, planes alimenticios, consumo diario ni reportes. El módulo administrativo solo incluye por ahora la revisión de productos.

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

### Administración de productos — SQL Server

| Método | Ruta | Descripción |
| --- | --- | --- |
| `GET` | `/api/administracion/productos/pendientes` | Lista productos pendientes de aprobación. |
| `PUT` | `/api/administracion/productos/{idProducto}/aprobacion` | Aprueba un producto pendiente. |

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

## Próximos pasos

1. Implementar Auth con contraseñas hasheadas y JWT.
2. Agregar clientes y nutricionistas como módulos relacionales.
3. Incorporar pruebas automatizadas cuando el entorno de desarrollo incluya el SDK de .NET.
4. Mantener scripts, vistas, funciones y procedimientos de base de datos fuera del alcance de los incrementos exclusivos del API.


Detalle importante de ir actualizando el readme a medida que se avanza en el desarrollo para mantener la documentación alineada con el estado actual del proyecto. Esto facilitará la colaboración y el onboarding de nuevos desarrolladores.