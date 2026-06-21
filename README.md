# NutriTEC

NutriTEC es una plataforma para acompañar hábitos de nutrición saludable. El repositorio incluye APIs, una web de cliente en HTML/JS, un backend prototipo de la vista cliente y una app móvil MAUI.

## Estado general actualizado

| Área | Estado | Notas |
| --- | --- | --- |
| API SQL Server | Implementada parcialmente y activa | Autenticación/JWT, productos, administración de aprobación, endpoints de Cliente y Nutricionista, objetos SQL programables. |
| API MongoDB | Implementada parcialmente y activa | Retroalimentaciones/foro entre paciente y nutricionista con JWT. |
| Web Cliente | Implementada como prototipo funcional local | HTML, CSS y JavaScript vanilla; usa `localStorage`, no consume todavía la API SQL central. |
| Backend en `Vista Cliente/backend` | Prototipo separado | Contiene modelos y `DbContext`, pero no es el backend principal recomendado; la API principal es `Api/NutriTec.SqlApi`. |
| App móvil MAUI | Implementada como prototipo funcional local | Login/registro, consumo diario y recetas con persistencia JSON local; todavía sin sincronización con APIs. |
| Vista Admin | Cubierta en API SQL | Productos pendientes y aprobación. No hay frontend admin dedicado en este repositorio. |
| Vista Nutricionista Web | No existe frontend dedicado | La API SQL ya expone endpoints para pacientes, planes y seguimiento. |

## Arquitectura principal

```text
Controller → Application Service → Repository → Database
```

```text
NutriTEC/
├── Api/
│   ├── NutriTec.SqlApi        # API principal relacional
│   └── NutriTec.MongoApi      # API documental para retroalimentaciones
├── Core/
│   ├── NutriTec.Application   # Casos de uso e interfaces
│   ├── NutriTec.Domain        # Entidades de dominio
│   └── NutriTec.Contracts     # DTOs públicos
├── Infrastructure/
│   ├── NutriTec.Infrastructure.Sql
│   └── NutriTec.Infrastructure.Mongo
├── Database/
│   ├── SqlServer/
│   └── MongoDB/
├── Vista Cliente/
│   ├── frontend/              # Web cliente prototipo
│   └── backend/               # Backend prototipo legado/separado
├── Mobile/Nutri-TEC/          # App MAUI prototipo
└── Tests/
```

Cada API registra únicamente su adaptador de persistencia:

- `NutriTec.SqlApi` usa `AddNutriTecSqlApplication()` y `AddNutriTecSqlInfrastructure()`.
- `NutriTec.MongoApi` usa `AddNutriTecMongoApplication()` y `AddNutriTecMongoInfrastructure()`.

## Requisitos locales

- .NET SDK compatible con los proyectos versionados.
- SQL Server accesible para ejecutar `Api/NutriTec.SqlApi`.
- MongoDB accesible para ejecutar `Api/NutriTec.MongoApi`.
- Navegador moderno para la web cliente.
- Workload MAUI/emulador si se desea ejecutar la app móvil.

> Nota de entorno: en este contenedor de trabajo no está instalado el CLI `dotnet`, por lo que aquí no se pueden ejecutar builds ni pruebas .NET.

## Ejecutar las APIs principales

```bash
dotnet restore NutriTec.slnx
dotnet run --project Api/NutriTec.SqlApi/NutriTec.SqlApi.csproj
dotnet run --project Api/NutriTec.MongoApi/NutriTec.MongoApi.csproj
```

## Autenticación y seguridad

La API SQL ya incluye:

- `POST /api/auth/login`
- `POST /api/auth/registrar-cliente`
- `POST /api/auth/registrar-nutricionista`
- `GET /api/auth/me`
- JWT Bearer con políticas de rol `Cliente`, `Nutricionista` y `Administrador`.
- Rate limit para login.
- Hashing de contraseñas en infraestructura SQL.

No se deben versionar contraseñas, secretos JWT reales, tarjetas completas ni cadenas de conexión productivas.

## Endpoints implementados en API SQL

### Productos y administración

| Método | Ruta | Descripción |
| --- | --- | --- |
| `POST` | `/api/productos` | Registra un producto pendiente de aprobación. |
| `GET` | `/api/productos` | Lista solo productos aprobados. |
| `GET` | `/api/productos/{idProducto}` | Consulta un producto por identificador. |
| `GET` | `/api/productos/buscar?nombre={nombre}` | Busca solo productos aprobados por nombre. |
| `GET` | `/api/productos/codigo-barras/{codigoBarras}` | Consulta solo productos aprobados por código de barras. |
| `PUT` | `/api/productos/{idProducto}` | Edita un producto. |
| `DELETE` | `/api/productos/{idProducto}` | Elimina un producto. |
| `GET` | `/api/administracion/productos/pendientes` | Lista productos pendientes de aprobación. |
| `PUT` | `/api/administracion/productos/{idProducto}/aprobacion` | Aprueba un producto pendiente. |

### Vista Cliente API

| Método | Ruta | Descripción |
| --- | --- | --- |
| `GET` | `/api/planes/usuario/{idUsuario}` | Planes asignados al cliente. |
| `POST` | `/api/registros-diarios` | Crear consumo diario. |
| `GET` | `/api/registros-diarios/usuario/{idUsuario}` | Historial de consumo diario. |
| `POST` | `/api/recetas` | Crear receta. |
| `GET` | `/api/recetas/usuario/{idUsuario}` | Recetas del usuario. |
| `GET` | `/api/medidas/usuario/{idUsuario}` | Historial de medidas. |
| `POST` | `/api/medidas/usuario/{idUsuario}` | Registrar medidas como cliente. |

### Vista Nutricionista API

| Método | Ruta | Descripción |
| --- | --- | --- |
| `GET` | `/api/pacientes/nutricionista/{cedula}` | Listar pacientes asociados. |
| `POST` | `/api/pacientes/asociar` | Asociar cliente como paciente. |
| `GET` | `/api/planes/nutricionista/{cedula}` | Listar planes del nutricionista. |
| `POST` | `/api/planes` | Crear plan alimenticio. |
| `POST` | `/api/planes/{idPlan}/tiempos-comida` | Agregar tiempos de comida al plan. |
| `GET` | `/api/registro-diario/paciente/{idUsuario}` | Ver registro diario del paciente. |

## Endpoints implementados en API MongoDB

| Método | Ruta | Descripción |
| --- | --- | --- |
| `POST` | `/api/retroalimentaciones` | Abre un foro con su primer mensaje. |
| `GET` | `/api/retroalimentaciones/pacientes/{idPaciente}` | Consulta foros de un paciente. |
| `GET` | `/api/retroalimentaciones/nutricionistas/{idNutricionista}` | Consulta foros de un nutricionista. |
| `POST` | `/api/retroalimentaciones/{idRetroalimentacion}/mensajes` | Agrega una respuesta a un foro. |

## Webs y apps implementadas

- **Web Cliente:** sí está implementada como prototipo local en `Vista Cliente/frontend`. Permite login/registro simulado, dashboard, medidas, perfil, recetas, reportes y registro de consumo usando `localStorage`.
- **Web Admin:** no hay frontend admin dedicado. La capacidad admin existe por API SQL.
- **Web Nutricionista:** no hay frontend nutricionista dedicado. La capacidad nutricionista existe por API SQL.
- **App móvil:** sí está implementada como prototipo MAUI en `Mobile/Nutri-TEC`; trabaja con JSON local y no sincroniza aún con las APIs.

## Documentación interna

Cada carpeta principal mantiene un README con su responsabilidad, estado y restricciones. Cuando se agregue una carpeta nueva relevante, se debe agregar un README breve y mantener esta tabla de estado actualizada.

## Pendientes conocidos

- Conectar la Web Cliente y la app MAUI con `Api/NutriTec.SqlApi` en lugar de usar almacenamiento local.
- Crear frontends dedicados para Admin y Nutricionista si el alcance del proyecto los requiere.
- Mover cualquier endpoint transitorio que consulte SQL desde controller hacia Application/Infrastructure para mantener la arquitectura estricta.
- Ejecutar builds/pruebas en un ambiente con SDK .NET instalado.
