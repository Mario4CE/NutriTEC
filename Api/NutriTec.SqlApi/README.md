# NutriTec.SqlApi

Proyecto ASP.NET Core encargado de exponer los módulos respaldados por SQL Server.

Responsabilidades principales:

- Configurar controllers para recursos relacionales.
- Registrar la capa `Application` y la infraestructura SQL mediante Dependency Injection.
- Publicar endpoints para productos y administración inicial.
- Aplicar middleware de errores de aplicación.

Restricciones:

- No debe depender de la infraestructura Mongo.
- No debe acceder a `DbContext` desde controllers.
- No debe implementar lógica de negocio dentro de endpoints.

## Configuración local y puertos

Esta API se ejecuta de forma local con `dotnet run` y usa los perfiles definidos en `Properties/launchSettings.json` cuando se inicia desde el proyecto.

Puertos configurados actualmente:

- Perfil `http`: `http://localhost:5255`.
- Perfil `https`: `https://localhost:7281` y `http://localhost:5255`.

Comando recomendado desde la raíz del repositorio:

```bash
dotnet run --project Api/NutriTec.SqlApi/NutriTec.SqlApi.csproj
```

Si otro desarrollador necesita cambiar los puertos locales, puede hacerlo en `Api/NutriTec.SqlApi/Properties/launchSettings.json`, modificando la propiedad `applicationUrl` del perfil correspondiente.

También puede sobrescribir temporalmente la URL sin editar archivos versionados usando `ASPNETCORE_URLS`:

```bash
ASPNETCORE_URLS=http://localhost:5001 dotnet run --project Api/NutriTec.SqlApi/NutriTec.SqlApi.csproj
```

## Configuración que puede cambiar cada desarrollador

Cada desarrollador puede ajustar localmente:

- Puertos de ejecución local.
- Perfil de ejecución (`http` o `https`).
- Variable `ASPNETCORE_ENVIRONMENT`, normalmente `Development` para trabajo local.
- Cadenas de conexión locales usando configuración segura o variables de entorno.

No se deben quemar secretos reales en archivos versionados. Si una configuración contiene contraseñas, tokens o credenciales, debe manejarse con variables de entorno, secretos de usuario de .NET o el mecanismo seguro definido por el equipo.

## Límites de cambio para esta API

Al modificar este proyecto se debe mantener el flujo:

```text
Controller → Service → Repository → Database
```

Por lo tanto:

- Los controllers no deben consultar directamente la base de datos.
- Los controllers no deben exponer entidades de infraestructura ni entidades con datos sensibles.
- Los contratos públicos deben mantenerse como DTOs separados.
- La API SQL no debe depender de MongoDB ni mezclar modelos documentales.
- No se debe agregar autenticación JWT sin un incremento explícito para ese objetivo.
- No se debe agregar `UseAuthentication()` si antes no existe una configuración completa de autenticación.

## Verificación básica

Para validar que la API inicia correctamente:

```bash
dotnet build
dotnet run --project Api/NutriTec.SqlApi/NutriTec.SqlApi.csproj
```

En ambiente `Development`, la especificación OpenAPI se publica con `MapOpenApi()` y puede consultarse en una ruta como:

```text
http://localhost:5255/openapi/v1.json
```
