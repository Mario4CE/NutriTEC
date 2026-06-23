# Pruebas desde Swagger UI

Swagger UI permite probar las APIs desde el navegador sin usar Postman ni archivos `.http`.

## URLs

Con las APIs levantadas en ambiente `Development`:

- SQL API: <http://localhost:5255/swagger>
- Mongo API: <http://localhost:5272/swagger>

## Flujo recomendado

1. Levante SQL API:

   ```bash
   dotnet run --project Api/NutriTec.SqlApi/NutriTec.SqlApi.csproj
   ```

2. Levante Mongo API:

   ```bash
   dotnet run --project Api/NutriTec.MongoApi/NutriTec.MongoApi.csproj
   ```

3. Abra Swagger SQL en `http://localhost:5255/swagger`.

4. Ejecute `POST /api/Auth/login` con un usuario válido.

5. Copie el JWT devuelto por login.

6. Presione el botón `Authorize` en Swagger y pegue solo el token, sin escribir `Bearer`.

7. Ejecute endpoints protegidos.

## Ejemplos SQL API

### Login

Endpoint:

```http
POST /api/Auth/login
```

Body:

```json
{
  "correo": "admin@nutritec.local",
  "contrasena": "Cambiar123!"
}
```

### Calcular IMC

Endpoint:

```http
GET /api/sql-programable/functions/imc?pesoKg=70&estaturaCm=175
```

Resultado esperado aproximado:

```json
{
  "success": true,
  "data": {
    "pesoKg": 70,
    "estaturaCm": 175,
    "imc": 22.86
  }
}
```

### Reporte de cobro

Endpoint:

```http
GET /api/sql-programable/stored-procedures/reporte-cobro-nutricionistas?montoBasePorPaciente=10000&incluirSinPacientes=true
```

Si no hay nutricionistas en la base, puede devolver una lista vacía.

### Registrar medida

Endpoint:

```http
POST /api/sql-programable/stored-procedures/usuarios/1/medidas
```

Body:

```json
{
  "fecha": "2026-06-20",
  "pesoKg": 70.5,
  "estaturaCm": 175,
  "cintura": 80,
  "cuello": 38,
  "caderas": 95,
  "pctMusculo": 42,
  "pctGrasa": 18
}
```

## Ejemplos Mongo API

Abra Swagger Mongo en `http://localhost:5272/swagger`, presione `Authorize` y pegue el JWT.

### Crear retroalimentación

Endpoint:

```http
POST /api/retroalimentaciones
```

Body:

```json
{
  "idPaciente": "11111111-1111-1111-1111-111111111111",
  "idNutricionista": "22222222-2222-2222-2222-222222222222",
  "autor": "Paciente",
  "mensaje": "Prueba desde Swagger UI."
}
```

### Consultar por paciente

Endpoint:

```http
GET /api/retroalimentaciones/pacientes/11111111-1111-1111-1111-111111111111
```

### Responder retroalimentación

Endpoint:

```http
POST /api/retroalimentaciones/{idRetroalimentacion}/mensajes
```

Body:

```json
{
  "autor": "Nutricionista",
  "mensaje": "Respuesta desde Swagger UI."
}
```
