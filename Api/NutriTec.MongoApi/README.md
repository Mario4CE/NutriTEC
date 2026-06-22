# NutriTec.MongoApi

Proyecto ASP.NET Core encargado de exponer los módulos respaldados por MongoDB.

## Estado actual

La API MongoDB está implementada para retroalimentaciones/foros entre pacientes y nutricionistas. Requiere JWT compatible con la API SQL y no reemplaza los módulos relacionales de cliente, nutricionista, productos o planes.

Responsabilidades principales:

- Configurar controllers para recursos documentales.
- Registrar la capa `Application` y la infraestructura Mongo mediante Dependency Injection.
- Publicar endpoints de retroalimentaciones.
- Aplicar middleware de validación y errores esperados.
- Validar JWT en endpoints documentales protegidos.

Restricciones:

- No debe depender de la infraestructura SQL.
- No debe acceder a colecciones Mongo desde controllers.
- No debe mezclar modelos documentales con modelos relacionales.

## Configuración local y puertos

Esta API se ejecuta de forma local con `dotnet run` y usa los perfiles definidos en `Properties/launchSettings.json` cuando se inicia desde el proyecto.

Puertos configurados actualmente:

- Perfil `http`: `http://localhost:5272`.
- Perfil `https`: `https://localhost:7044` y `http://localhost:5272`.

Comando recomendado desde la raíz del repositorio:

```bash
dotnet run --project Api/NutriTec.MongoApi/NutriTec.MongoApi.csproj
```

Si otro desarrollador necesita cambiar los puertos locales, puede hacerlo en `Api/NutriTec.MongoApi/Properties/launchSettings.json`, modificando la propiedad `applicationUrl` del perfil correspondiente.

También puede sobrescribir temporalmente la URL sin editar archivos versionados usando `ASPNETCORE_URLS`:

```bash
ASPNETCORE_URLS=http://localhost:5002 dotnet run --project Api/NutriTec.MongoApi/NutriTec.MongoApi.csproj
```

## Configuración que puede cambiar cada desarrollador

Cada desarrollador puede ajustar localmente:

- Puertos de ejecución local.
- Perfil de ejecución (`http` o `https`).
- Variable `ASPNETCORE_ENVIRONMENT`, normalmente `Development` para trabajo local.
- Configuración local de MongoDB usando valores seguros o variables de entorno.

No se deben quemar secretos reales en archivos versionados. Si una configuración contiene contraseñas, tokens o credenciales, debe manejarse con variables de entorno, secretos de usuario de .NET o el mecanismo seguro definido por el equipo.

## Límites de cambio para esta API

Al modificar este proyecto se debe mantener el flujo:

```text
Controller → Service → Repository → Database
```

Por lo tanto:

- Los controllers no deben consultar directamente MongoDB.
- Los controllers no deben exponer entidades de infraestructura ni documentos internos con datos sensibles.
- Los contratos públicos deben mantenerse como DTOs separados.
- La API Mongo no debe depender de SQL Server ni mezclar modelos relacionales.
- La autenticación JWT ya está configurada para proteger retroalimentaciones.
- Cualquier nuevo endpoint protegido debe usar `Authorization: Bearer <jwt>` y mantener las validaciones de issuer, audience, signing key y lifetime.
- No se deben guardar secretos reales de JWT en archivos versionados; usar variables de entorno, user-secrets o gestor de secretos.


## Autenticación y endpoints de retroalimentaciones

Los endpoints de retroalimentaciones requieren JWT porque pueden contener comunicación entre pacientes y nutricionistas. El token se envía así:

```http
Authorization: Bearer <jwt>
```

Flujo básico de uso:

```http
POST /api/retroalimentaciones
GET /api/retroalimentaciones/pacientes/{idPaciente}
GET /api/retroalimentaciones/nutricionista/{idNutricionista}
POST /api/retroalimentaciones/{idRetroalimentacion}/mensajes
```

La autenticación se comparte conceptualmente con la API SQL: el token debe estar firmado con la configuración `Jwt` compatible entre APIs.

## Verificación básica

Para validar que la API inicia correctamente:

```bash
dotnet build
dotnet run --project Api/NutriTec.MongoApi/NutriTec.MongoApi.csproj
```

En ambiente `Development`, Swagger UI se publica con Swashbuckle y puede consultarse en:

```text
http://localhost:5272/swagger
```
