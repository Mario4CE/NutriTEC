# Pruebas Swagger — Endpoints de Vistas Cliente y Nutricionista

Este archivo sirve como guía manual para probar en Swagger los endpoints agregados en `VistasController`.

## 0. Preparar datos

Antes de abrir Swagger, ejecute en SQL Server al menos uno de estos seeds:

```powershell
sqlcmd -S .\SQLEXPRESS -d NutriTec -i Tests\SeedDemoData.sql
```

Opcional para más clientes:

```powershell
sqlcmd -S .\SQLEXPRESS -d NutriTec -i Tests\SeedExtendedClients.sql
```

Para revisar datos cargados:

```powershell
sqlcmd -S .\SQLEXPRESS -d NutriTec -i Tests\VerifyDemoData.sql
```

## 1. Levantar API SQL y abrir Swagger

```powershell
dotnet run --project Api\NutriTec.SqlApi\NutriTec.SqlApi.csproj
```

Abra:

```text
http://localhost:5255/swagger
```

## 2. Login y autorización en Swagger

### Cliente demo

Endpoint:

```http
POST /api/auth/login
```

Body:

```json
{
  "correo": "cliente.demo1@nutritec.test",
  "contrasena": "Cliente2026!"
}
```

Copie el valor `token` de la respuesta. Luego haga clic en **Authorize** en Swagger y pegue:

```text
Bearer <token-cliente>
```

> En algunos Swagger UI solo debe pegar el token sin la palabra `Bearer`. Si falla con `Bearer`, péguelo sin prefijo.

### Nutricionista demo

Endpoint:

```http
POST /api/auth/login
```

Body:

```json
{
  "correo": "nutri.demo1@nutritec.test",
  "contrasena": "Nutricion2026!"
}
```

Use este token para probar endpoints con rol `Nutricionista`.

## 3. Datos fijos útiles del seed

Productos demo aprobados:

```text
10000000-0000-0000-0000-000000000001 — Avena integral demo
10000000-0000-0000-0000-000000000002 — Yogurt natural demo
10000000-0000-0000-0000-000000000003 — Pechuga de pollo demo
10000000-0000-0000-0000-000000000004 — Arroz integral demo
10000000-0000-0000-0000-000000000005 — Manzana roja demo
```

Nutricionista demo:

```text
cedula: NUT-DEMO-001
correo: nutri.demo1@nutritec.test
password: Nutricion2026!
```

Cliente demo:

```text
correo: cliente.demo1@nutritec.test
password: Cliente2026!
```

Para saber el `idUsuario` real del cliente, use la respuesta de `POST /api/auth/login` o consulte:

```sql
SELECT id_usuario, nombre, apellidos, email
FROM USUARIO
WHERE email = 'cliente.demo1@nutritec.test';
```

En los ejemplos siguientes reemplace `1` por el `idUsuario` real.

---

# Vista Cliente

## 4. GET /api/productos

Objetivo: confirmar que solo devuelve productos aprobados.

En Swagger:

```http
GET /api/productos
```

Resultado esperado:

- Código `200`.
- Lista de productos.
- Los productos deben tener estado aprobado.

## 5. GET /api/planes/usuario/{idUsuario}

En Swagger:

```http
GET /api/planes/usuario/1
```

Reemplace `1` por el `idUsuario` real.

Resultado esperado:

- Código `200`.
- Lista de planes asignados si el seed cargó asignaciones.

## 6. GET /api/registros-diarios/usuario/{idUsuario}

```http
GET /api/registros-diarios/usuario/1
```

Resultado esperado:

- Código `200`.
- Registros diarios del cliente.

## 7. POST /api/registros-diarios

```http
POST /api/registros-diarios
```

Body:

```json
{
  "idUsuario": 1,
  "fecha": "2026-06-21",
  "tipoComida": "Desayuno",
  "productos": [
    {
      "idProducto": "10000000-0000-0000-0000-000000000001",
      "cantidadPorciones": 1.00
    },
    {
      "idProducto": "10000000-0000-0000-0000-000000000005",
      "cantidadPorciones": 2.00
    }
  ]
}
```

Resultado esperado:

- Código `201`.
- Respuesta con `idRegistro`.

## 8. GET /api/recetas/usuario/{idUsuario}

```http
GET /api/recetas/usuario/1
```

Resultado esperado:

- Código `200`.
- Recetas del cliente.

## 9. POST /api/recetas

```http
POST /api/recetas
```

Body:

```json
{
  "idUsuario": 1,
  "nombre": "Batido de prueba Swagger",
  "productos": [
    {
      "idProducto": "10000000-0000-0000-0000-000000000002",
      "cantidadPorciones": 1.00
    },
    {
      "idProducto": "10000000-0000-0000-0000-000000000005",
      "cantidadPorciones": 1.50
    }
  ]
}
```

Resultado esperado:

- Código `201`.
- Respuesta con `idReceta`.

## 10. GET /api/medidas/usuario/{idUsuario}

```http
GET /api/medidas/usuario/1
```

Resultado esperado:

- Código `200`.
- Historial de medidas.

## 11. POST /api/medidas/usuario/{idUsuario}

```http
POST /api/medidas/usuario/1
```

Body:

```json
{
  "fecha": "2026-06-21",
  "cintura": 81.50,
  "cuello": 37.00,
  "caderas": 95.00,
  "pctMusculo": 42.50,
  "pctGrasa": 19.50
}
```

Resultado esperado:

- Código `201`.
- Respuesta con `idMedida`.

---

# Vista Nutricionista

Para estos endpoints debe autorizar Swagger con el token del nutricionista.

## 12. GET /api/pacientes/nutricionista/{cedula}

```http
GET /api/pacientes/nutricionista/NUT-DEMO-001
```

Resultado esperado:

- Código `200`.
- Pacientes asociados a `NUT-DEMO-001`.

## 13. POST /api/pacientes/asociar

Use un cliente que no esté asociado o uno creado por `SeedExtendedClients.sql`, por ejemplo `ext.cliente09@nutritec.test`.

Primero obtenga su `id_usuario` en SQL:

```sql
SELECT id_usuario, email
FROM USUARIO
WHERE email = 'ext.cliente09@nutritec.test';
```

Luego en Swagger:

```http
POST /api/pacientes/asociar
```

Body:

```json
{
  "cedulaNutricionista": "NUT-DEMO-001",
  "idUsuario": 9
}
```

Reemplace `9` por el `id_usuario` real.

Resultado esperado:

- Código `201`.
- Asociación creada.

## 14. GET /api/planes/nutricionista/{cedula}

```http
GET /api/planes/nutricionista/NUT-DEMO-001
```

Resultado esperado:

- Código `200`.
- Planes creados por el nutricionista.

## 15. POST /api/planes

```http
POST /api/planes
```

Body:

```json
{
  "nombre": "Plan Swagger de mantenimiento",
  "cedulaNutricionista": "NUT-DEMO-001"
}
```

Resultado esperado:

- Código `201`.
- Respuesta con `idPlan`.

Guarde ese `idPlan` para la siguiente prueba.

## 16. POST /api/planes/{idPlan}/tiempos-comida

```http
POST /api/planes/1/tiempos-comida
```

Reemplace `1` por el `idPlan` creado en el paso anterior.

Body:

```json
{
  "tipoComida": "Almuerzo",
  "productos": [
    {
      "idProducto": "10000000-0000-0000-0000-000000000003",
      "cantidadPorciones": 1.00
    },
    {
      "idProducto": "10000000-0000-0000-0000-000000000004",
      "cantidadPorciones": 1.50
    }
  ]
}
```

Resultado esperado:

- Código `201`.
- Respuesta con `idTiempo`.

## 17. GET /api/registro-diario/paciente/{idUsuario}

```http
GET /api/registro-diario/paciente/1
```

Reemplace `1` por el `idUsuario` real de un paciente.

Resultado esperado:

- Código `200`.
- Registros diarios del paciente.

---

# Errores comunes

## 401 Unauthorized

Causa probable:

- No se pegó el token en Swagger.
- Se usó token de Cliente para endpoint de Nutricionista.
- Se usó token de Nutricionista para endpoint de Cliente.

Solución:

- Haga login con el rol correcto.
- Use **Authorize** en Swagger.

## 403 Forbidden

Causa probable:

- El token existe, pero el rol no coincide con la política requerida.

Solución:

- Cliente para endpoints de Cliente.
- Nutricionista para endpoints de Nutricionista.

## 404 o listas vacías

Causa probable:

- No ejecutó `Tests/SeedDemoData.sql`.
- Usó un `idUsuario` incorrecto.
- Usó una `cedulaNutricionista` que no existe.

Solución:

- Ejecutar `Tests/SeedDemoData.sql`.
- Ejecutar `Tests/VerifyDemoData.sql`.
- Revisar IDs reales en SQL.
