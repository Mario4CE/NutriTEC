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

## Despliegue recomendado en Azure

Para cumplir el requisito de que la capa de servicios esté en **C# + Entity Framework** y que la API, la web y la base de datos estén desplegadas en la nube, la opción más directa es usar estos servicios de Azure:

| Componente del proyecto | Servicio de Azure | Proyecto/carpeta |
| --- | --- | --- |
| Capa de servicios/API C# | Azure App Service para ASP.NET Core | `Api/NutriTec.SqlApi` |
| Base de datos relacional | Azure SQL Database | Scripts de `Database/SqlServer` y EF Core de `Infrastructure/NutriTec.Infrastructure.Sql` |
| Web admin React/Vite | Azure Static Web Apps o Azure App Service | `nutritec-admin` |
| Web cliente HTML/JS | Azure Static Web Apps | `Vista Cliente/frontend` |
| MongoDB/retroalimentaciones, si se incluye | Azure Cosmos DB for MongoDB o MongoDB Atlas | `Api/NutriTec.MongoApi` |

### ¿Azure para un API o para una web?

Sí: en Azure se publican ambos, pero normalmente en servicios distintos.

- Un **API ASP.NET Core** se publica en **Azure App Service**. El resultado es una URL tipo `https://nutritec-sql-api.azurewebsites.net` que expone rutas como `/api/auth/login` o `/api/productos`.
- Una **web estática** como React/Vite o HTML/JS se publica en **Azure Static Web Apps**. Esa web llama al API usando la URL pública del App Service.
- La **base de datos** va en **Azure SQL Database**. El API no debe guardar la cadena de conexión en el repositorio; se configura como variable de aplicación en Azure.

### Variables que se configuran en Azure App Service para el API SQL

En el portal de Azure, entrar al App Service del API y abrir **Settings → Environment variables**. Agregar:

| Nombre | Ejemplo | Descripción |
| --- | --- | --- |
| `ConnectionStrings__NutriTec` | `Server=tcp:<servidor>.database.windows.net,1433;Initial Catalog=NutriTEC;User ID=<usuario>;Password=<password>;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;` | Cadena de conexión usada por EF Core. |
| `Jwt__Issuer` | `NutriTec.SqlApi` | Emisor del token. |
| `Jwt__Audience` | `NutriTec.Clients` | Audiencia del token. |
| `Jwt__Secret` | un secreto largo, no versionado | Llave para firmar JWT; obligatoria en producción. |
| `Cors__AllowedOrigins__0` | `https://<tu-web>.azurestaticapps.net` | Permite que la web desplegada consuma el API. |
| `BootstrapAdmin__Enabled` | `true` o `false` | Activa la creación inicial del administrador si se requiere. |
| `BootstrapAdmin__Email` | `admin@nutritec.com` | Correo del admin inicial si el bootstrap está activo. |

> En ASP.NET Core, las variables con doble guion bajo (`__`) reemplazan los niveles de `appsettings.json`. Por ejemplo, `ConnectionStrings__NutriTec` reemplaza `ConnectionStrings:NutriTec`.

### Pasos rápidos con Azure CLI

Los nombres deben ser únicos globalmente donde aplique. Cambiar `nutritec-demo` por un prefijo propio.

```bash
# 1) Login y variables base
az login
az group create --name rg-nutritec --location eastus

# 2) Crear Azure SQL Server y base de datos
az sql server create \
  --name sql-nutritec-demo \
  --resource-group rg-nutritec \
  --location eastus \
  --admin-user nutritecadmin \
  --admin-password "CAMBIAR_PASSWORD_SEGURA"

az sql db create \
  --resource-group rg-nutritec \
  --server sql-nutritec-demo \
  --name NutriTEC \
  --service-objective Basic

# Permitir acceso desde servicios Azure al SQL Server
az sql server firewall-rule create \
  --resource-group rg-nutritec \
  --server sql-nutritec-demo \
  --name AllowAzureServices \
  --start-ip-address 0.0.0.0 \
  --end-ip-address 0.0.0.0

# 3) Crear App Service para el API C#
az appservice plan create \
  --name plan-nutritec-api \
  --resource-group rg-nutritec \
  --sku B1 \
  --is-linux

az webapp create \
  --name app-nutritec-sql-api-demo \
  --resource-group rg-nutritec \
  --plan plan-nutritec-api \
  --runtime "DOTNET:8"

# 4) Configurar secretos del API en Azure
az webapp config appsettings set \
  --name app-nutritec-sql-api-demo \
  --resource-group rg-nutritec \
  --settings \
    Jwt__Issuer="NutriTec.SqlApi" \
    Jwt__Audience="NutriTec.Clients" \
    Jwt__Secret="CAMBIAR_SECRETO_LARGO_DE_PRODUCCION" \
    Cors__AllowedOrigins__0="https://<tu-web>.azurestaticapps.net"

az webapp config connection-string set \
  --name app-nutritec-sql-api-demo \
  --resource-group rg-nutritec \
  --connection-string-type SQLAzure \
  --settings NutriTec="Server=tcp:sql-nutritec-demo.database.windows.net,1433;Initial Catalog=NutriTEC;User ID=nutritecadmin;Password=CAMBIAR_PASSWORD_SEGURA;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"

# 5) Publicar el API desde la carpeta del repo
dotnet publish Api/NutriTec.SqlApi/NutriTec.SqlApi.csproj -c Release -o ./publish/sql-api
cd publish/sql-api
zip -r ../sql-api.zip .
cd ../..
az webapp deploy \
  --resource-group rg-nutritec \
  --name app-nutritec-sql-api-demo \
  --src-path ./publish/sql-api.zip \
  --type zip
```

### Crear tablas y objetos en Azure SQL

Se puede ejecutar el script completo desde SQL Server Management Studio, Azure Data Studio o `sqlcmd` apuntando a la base de datos de Azure SQL:

```bash
sqlcmd \
  -S tcp:sql-nutritec-demo.database.windows.net,1433 \
  -d NutriTEC \
  -U nutritecadmin \
  -P "CAMBIAR_PASSWORD_SEGURA" \
  -i Database/SqlServer/Complete/TablaCompleta.sql
```

Si se desea usar migraciones de EF Core en vez de scripts SQL, ejecutar el comando contra la misma cadena de conexión de Azure desde un ambiente con .NET SDK y `dotnet-ef` instalado.

### Publicar la web admin React/Vite

Para la web de `nutritec-admin`, configurar primero la URL del API. En desarrollo el proyecto usa proxy `/api/sql`, pero en Azure la web debe llamar al dominio público del App Service o usar una regla de proxy de Static Web Apps.

Opción simple: desplegar con Azure Static Web Apps desde GitHub indicando:

| Campo | Valor |
| --- | --- |
| App location | `nutritec-admin` |
| Output location | `dist` |
| Build command | `npm run build` |

Después, actualizar `Cors__AllowedOrigins__0` en el App Service con la URL real de Static Web Apps.

### Checklist para demostrar el requisito

- API SQL en C# publicada en Azure App Service y respondiendo `/swagger` o `/api/productos`.
- Azure SQL Database creada con las tablas, vistas, funciones, triggers y procedimientos almacenados del proyecto.
- `ConnectionStrings__NutriTec` configurada en Azure, no en Git.
- `Jwt__Secret` configurado como secreto de Azure, no en Git.
- `Cors__AllowedOrigins__0` apuntando a la URL de la web desplegada.
- Web admin o web cliente publicada en Azure Static Web Apps y consumiendo el API publicado.

## Documentación interna

Cada carpeta principal mantiene un README con su responsabilidad, estado y restricciones. Cuando se agregue una carpeta nueva relevante, se debe agregar un README breve y mantener esta tabla de estado actualizada.

## Pendientes conocidos

- Conectar la Web Cliente y la app MAUI con `Api/NutriTec.SqlApi` en lugar de usar almacenamiento local.
- Crear frontends dedicados para Admin y Nutricionista si el alcance del proyecto los requiere.
- Mover cualquier endpoint transitorio que consulte SQL desde controller hacia Application/Infrastructure para mantener la arquitectura estricta.
- Ejecutar builds/pruebas en un ambiente con SDK .NET instalado.
