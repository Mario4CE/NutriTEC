# NutriTec SQL API

API HTTP para los casos de uso que persisten en SQL Server. En esta etapa incluye los endpoints de autenticación para login y registro de clientes/nutricionistas.

## Ejecución local

La configuración de desarrollo usa LocalDB con Windows Authentication:

```text
Server=(localdb)\MSSQLLocalDB;Database=NutriTec;Trusted_Connection=True;TrustServerCertificate=True
```

Ejecutar la API SQL:

```bash
dotnet run --project Api/NutriTec.SqlApi/NutriTec.SqlApi.csproj
```

Si se requiere fijar puertos locales:

```bash
ASPNETCORE_URLS="http://localhost:5255;https://localhost:7281" dotnet run --project Api/NutriTec.SqlApi/NutriTec.SqlApi.csproj
```

## Endpoints de autenticación

Base URL local sugerida:

```text
http://localhost:5255
```

### Login

```http
POST /api/auth/login
Content-Type: application/json
```

Body:

```json
{
  "correo": "cliente@example.com",
  "contrasena": "Password123!"
}
```

Respuesta exitosa `200 OK`:

```json
{
  "idUsuario": "1",
  "nombre": "Cliente Demo",
  "correo": "cliente@example.com",
  "tipoUsuario": "Cliente",
  "token": "<jwt>",
  "expiraEn": "2026-06-15T18:00:00+00:00"
}
```

Credenciales inválidas `401 Unauthorized`:

```json
{
  "codigo": "credenciales_invalidas",
  "mensaje": "Correo o contraseña inválidos."
}
```

Límite de intentos excedido `429 Too Many Requests`:

```json
{
  "codigo": "rate_limit",
  "mensaje": "Demasiados intentos. Intente nuevamente más tarde."
}
```

El login aplica rate limiting por IP remota: máximo 5 intentos por minuto, sin cola. Cuando el runtime puede calcularlo, la respuesta incluye el encabezado `Retry-After`.

Las respuestas de autenticación bajo `/api/auth` agregan headers anti-cache para evitar almacenar JWT o datos sensibles:

```http
Cache-Control: no-store
Pragma: no-cache
Expires: 0
```

### Registrar cliente

```http
POST /api/auth/registrar-cliente
Content-Type: application/json
```

Body:

```json
{
  "nombre": "Cliente",
  "apellidos": "Demo",
  "edad": 30,
  "fechaNacimiento": "1994-01-20",
  "peso": 72.5,
  "imc": 24.1,
  "pais": "Costa Rica",
  "cintura": 82.0,
  "cuello": 38.0,
  "caderas": 95.0,
  "pctMusculo": 42.0,
  "pctGrasa": 18.5,
  "caloriasDiariasMax": 2200,
  "correo": "cliente@example.com",
  "contrasena": "Password123!"
}
```

Respuesta exitosa `201 Created` devuelve `LoginResponse` con JWT, sin contraseña ni `password_hash`.

Correo duplicado `409 Conflict`:

```json
{
  "codigo": "conflicto",
  "mensaje": "El correo ya está registrado."
}
```

### Registrar nutricionista

```http
POST /api/auth/registrar-nutricionista
Content-Type: application/json
```

Body:

```json
{
  "cedula": "1-1111-1111",
  "nombre": "Nutricionista",
  "apellidos": "Demo",
  "codigoNutricionista": "NUT-001",
  "edad": 35,
  "fechaNacimiento": "1989-05-10",
  "peso": 68.0,
  "imc": 23.0,
  "direccion": "San José, Costa Rica",
  "fotoUrl": null,
  "tarjetaCredito": "4111111111111111",
  "tipoCobro": "mensual",
  "correo": "nutricionista@example.com",
  "contrasena": "Password123!"
}
```

La API no almacena la tarjeta completa: antes de persistirla la convierte a un valor enmascarado como `****-****-****-1111`. No se debe enviar CVV ni usar tarjetas reales.

`tipoCobro` debe ser uno de:

- `semanal`
- `mensual`
- `anual`

Estos valores corresponden al catálogo SQL `TIPO_COBRO` y deben existir antes de registrar nutricionistas.

Respuesta exitosa `201 Created` devuelve `LoginResponse` con JWT, sin contraseña ni `password_hash`.

### Usuario autenticado

```http
GET /api/auth/me
Authorization: Bearer <jwt>
```

Respuesta exitosa `200 OK`:

```json
{
  "idUsuario": "1",
  "nombre": "Cliente Demo",
  "correo": "cliente@example.com",
  "tipoUsuario": "Cliente"
}
```

Este endpoint está protegido con JWT y devuelve únicamente claims de identidad ya validados. No consulta base de datos, no devuelve contraseña, no devuelve `password_hash` y no emite tokens nuevos.

## Autenticación JWT

El login y los registros devuelven un JWT firmado. Para consumir endpoints protegidos en futuros módulos, enviar el token así:

```http
Authorization: Bearer <jwt>
```

Claims incluidos inicialmente:

- `sub`: identificador del usuario.
- `email`: correo del usuario.
- `name`: nombre del usuario.
- `role`: tipo de usuario (`Cliente`, `Nutricionista` o `Administrador`).

La configuración base vive en `Jwt`. El valor de `Jwt:Secret` no debe ser un secreto real dentro del repositorio; para ambientes reales debe venir de variables de entorno, user-secrets o un gestor de secretos.

En producción, la API falla al iniciar si `Jwt:Secret` está vacío o no está configurado. El valor placeholder solo existe para evitar configuraciones rotas en escenarios locales controlados y no debe usarse en despliegues reales.

## Bootstrap de administrador inicial

La API puede crear de forma controlada el primer administrador al iniciar, pero esta opción está deshabilitada por defecto y debe usarse únicamente para preparar un ambiente donde todavía no existe una cuenta administrativa.

Configuración base versionada:

```json
{
  "BootstrapAdmin": {
    "Enabled": false,
    "Email": ""
  }
}
```

Para habilitarlo en un ambiente real o local controlado, usar variables de entorno, user-secrets o gestor de secretos:

```bash
BootstrapAdmin__Enabled=true
BootstrapAdmin__Email="admin@nutritec.example"
BootstrapAdmin__Password="<contraseña-temporal-segura>"
```

Reglas de seguridad:

- `BootstrapAdmin:Password` no debe guardarse en `appsettings.json`.
- El bootstrap solo crea un administrador si no existe ningún registro en `ADMINISTRADOR`.
- La contraseña temporal se procesa con `PasswordHasher` y solo se almacena `password_hash`.
- Después de crear el primer administrador, deshabilitar `BootstrapAdmin:Enabled`.
- No crea clientes, nutricionistas, productos ni datos de negocio.
- No reemplaza un módulo futuro de administración de usuarios.

## CORS restringido

La API SQL registra una política CORS restringida llamada `RestrictedCors`. Los orígenes permitidos se leen desde:

```json
{
  "Cors": {
    "AllowedOrigins": [
      "https://frontend.example.com"
    ]
  }
}
```

No se usa `AllowAnyOrigin`. Si `Cors:AllowedOrigins` queda vacío, las solicitudes cross-origin no quedan habilitadas para ningún dominio. Para ambientes reales, configurar los dominios mediante variables de entorno, user-secrets o el gestor de configuración del despliegue, por ejemplo:

```bash
Cors__AllowedOrigins__0="https://app.nutritec.example"
```

## Forwarded headers

La API SQL procesa `X-Forwarded-For` y `X-Forwarded-Proto` para escenarios detrás de proxy o load balancer, pero solo debe confiar en proxies conocidos. Las IPs permitidas se leen desde:

```json
{
  "ForwardedHeaders": {
    "KnownProxies": [
      "10.0.0.10"
    ]
  }
}
```

Si `ForwardedHeaders:KnownProxies` queda vacío, se mantienen los valores seguros por defecto del runtime y no se abre confianza global a cualquier cliente. Para ambientes reales, configurar únicamente IPs de proxies administrados mediante variables de entorno o configuración del despliegue, por ejemplo:

```bash
ForwardedHeaders__KnownProxies__0="10.0.0.10"
```

Esta configuración permite que funcionalidades como rate limiting por IP usen la IP real del cliente cuando la solicitud pasa por un proxy confiable.

## Estrategia de cache

La API debe aplicar cache solo cuando el endpoint sea seguro para reutilización y no contenga datos sensibles ni personalizados.

### No cachear

No se deben cachear respuestas que contengan o dependan de:

- JWT, login o registro.
- Contraseñas, `password_hash` o secretos.
- Datos personales o de salud.
- Datos de tarjeta o información de pago.
- Respuestas específicas de un usuario autenticado, salvo que exista una regla explícita y segura.

Los endpoints bajo `/api/auth` ya agregan headers `no-store` para evitar cache accidental de tokens y datos sensibles.

### Cache futuro permitido

Cuando existan endpoints adecuados, se puede evaluar cache para:

- Catálogos públicos o de baja variación.
- Productos aprobados de consulta pública.
- Configuración pública no sensible.
- Reportes agregados que no expongan información personal.

### Tipos de cache recomendados

- **Cliente/HTTP cache:** útil para recursos públicos y de baja sensibilidad usando headers como `Cache-Control` con `max-age` controlado.
- **`IMemoryCache`:** útil solo en despliegues de una instancia o para datos recreables.
- **Cache distribuido/Redis:** preferible cuando la API escale a múltiples instancias.

Toda cache debe tener un tiempo de vida definido. Como guía inicial: catálogos de baja variación pueden usar minutos u horas; datos sensibles deben usar `no-store`.

## Verificación básica

1. Confirmar que la base `NutriTec` existe en LocalDB y que las tablas SQL requeridas ya fueron creadas.
2. Ejecutar la API SQL.
3. Registrar un cliente.
4. Hacer login con el correo y contraseña registrados.
5. Intentar registrar el mismo correo nuevamente y confirmar respuesta `409 Conflict`.
6. Intentar login con contraseña incorrecta y confirmar respuesta `401 Unauthorized` con mensaje genérico.
7. Repetir intentos de login hasta exceder el límite y confirmar respuesta `429 Too Many Requests` con código `rate_limit`.
8. Consumir `GET /api/auth/me` con `Authorization: Bearer <jwt>` y confirmar que devuelve la identidad del usuario autenticado.

## Verificación opcional del bootstrap de administrador

El bootstrap de administrador debe probarse solo en ambientes locales/controlados y con credenciales temporales no reutilizadas.

1. Confirmar que la tabla `ADMINISTRADOR` está vacía.
2. Configurar `BootstrapAdmin__Enabled=true`, `BootstrapAdmin__Email` y `BootstrapAdmin__Password` fuera de `appsettings`.
3. Iniciar la API SQL.
4. Confirmar que se creó un único administrador.
5. Confirmar que la columna `password_hash` no contiene la contraseña temporal en texto plano.
6. Hacer login con el correo y contraseña temporal del administrador.
7. Confirmar que el JWT contiene rol `Administrador`.
8. Deshabilitar `BootstrapAdmin__Enabled` después de crear la cuenta inicial.
9. Reiniciar la API y confirmar que no se crea un segundo administrador.

## Límites arquitectónicos

- Los controllers dependen de servicios de Application, no de `DbContext`.
- Los DTOs públicos viven en `Core/NutriTec.Contracts`.
- Las entidades SQL viven en `Infrastructure` y no deben exponerse al frontend.
- No se devuelve `password_hash` ni contraseña desde el API.
- JWT está habilitado para emitir tokens en login y registro.
- `GET /api/auth/me` es un endpoint protegido para validar JWT y exponer solo claims seguros.
- El bootstrap de administrador inicial está deshabilitado por defecto y solo debe activarse con secretos externos al repositorio.
- El endpoint de login aplica rate limiting para reducir intentos abusivos de autenticación.
- Las respuestas de autenticación usan headers `no-store` para evitar cache accidental de tokens.
- CORS usa una política restringida por configuración y no permite cualquier origen.
- Forwarded headers solo debe confiar en proxies conocidos configurados por ambiente.
- La cache solo debe aplicarse a endpoints seguros, públicos o agregados, nunca a autenticación ni datos sensibles.
- No usar secretos reales en `appsettings.json`; usar variables de entorno o user-secrets para `Jwt:Secret` cuando aplique.
