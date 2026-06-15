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
  "mensaje": "Correo o contraseña inválidos."
}
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
  "tarjetaCredito": "411111******1111",
  "tipoCobro": "mensual",
  "correo": "nutricionista@example.com",
  "contrasena": "Password123!"
}
```

`tipoCobro` debe ser uno de:

- `semanal`
- `mensual`
- `anual`

Respuesta exitosa `201 Created` devuelve `LoginResponse` con JWT, sin contraseña ni `password_hash`.

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

## Verificación básica

1. Confirmar que la base `NutriTec` existe en LocalDB y que las tablas SQL requeridas ya fueron creadas.
2. Ejecutar la API SQL.
3. Registrar un cliente.
4. Hacer login con el correo y contraseña registrados.
5. Intentar registrar el mismo correo nuevamente y confirmar respuesta `409 Conflict`.
6. Intentar login con contraseña incorrecta y confirmar respuesta `401 Unauthorized` con mensaje genérico.

## Límites arquitectónicos

- Los controllers dependen de servicios de Application, no de `DbContext`.
- Los DTOs públicos viven en `Core/NutriTec.Contracts`.
- Las entidades SQL viven en `Infrastructure` y no deben exponerse al frontend.
- No se devuelve `password_hash` ni contraseña desde el API.
- JWT está habilitado para emitir tokens en login y registro.
- No usar secretos reales en `appsettings.json`; usar variables de entorno o user-secrets para `Jwt:Secret` cuando aplique.
