# NutriTec.MongoApi

Proyecto ASP.NET Core encargado de exponer los módulos respaldados por MongoDB.

Responsabilidades principales:

- Configurar controllers para recursos documentales.
- Registrar la capa `Application` y la infraestructura Mongo mediante Dependency Injection.
- Publicar endpoints de retroalimentaciones.
- Aplicar middleware de validación y errores esperados.

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
- No se debe agregar autenticación JWT sin un incremento explícito para ese objetivo.
- No se debe agregar `UseAuthentication()` si antes no existe una configuración completa de autenticación.

## Verificación básica

Para validar que la API inicia correctamente:

```bash
dotnet build
dotnet run --project Api/NutriTec.MongoApi/NutriTec.MongoApi.csproj
```

En ambiente `Development`, la especificación OpenAPI se publica con `MapOpenApi()` y puede consultarse en una ruta como:

```text
http://localhost:5272/openapi/v1.json
```
